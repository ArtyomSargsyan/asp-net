using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.DTO;
using ToDoApi.Models;
using ToDoApi.Repositories.RefreshTokens;
using ToDoApi.Repositories.Users;
using ToDoApi.Services.Auth;

namespace ToDoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    JwtService jwt,
    ILogger<AuthController> logger) : ControllerBase
{
    // ────────────────────────────────────────────────────────────────────────────
    // Register
    // ────────────────────────────────────────────────────────────────────────────

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        if (await userRepo.ExistsByUsernameAsync(dto.UserName))
            return Conflict("Username already exists.");

        var user = new User
        {
            UserName     = dto.UserName,
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        await userRepo.AddAsync(user);
        return Ok(new { Message = "User registered successfully." });
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Login
    // ────────────────────────────────────────────────────────────────────────────

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await userRepo.GetByUsernameAsync(dto.UserName);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var accessToken  = jwt.GenerateToken(user);
        var refreshToken = jwt.GenerateRefreshToken(user.Id);

        await refreshTokenRepo.AddAsync(refreshToken);

        return Ok(new AuthResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken.Token,
            UserName     = user.UserName,
        });
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Refresh  —  thread-safe token rotation with replay-attack detection
    //
    // Security model (OAuth 2.0 Security BCP §4.14 — Refresh Token Rotation):
    //   • Every successful refresh issues a NEW access + refresh token pair and
    //     atomically marks the used refresh token as consumed (IsRevoked=true, UsedAt=now).
    //   • If a consumed token is presented again it proves the token family has been
    //     compromised: all active sessions for that user are immediately revoked.
    //   • Thread safety is achieved with a single atomic SQL UPDATE that only
    //     matches rows where IsRevoked = false.  Only ONE concurrent request can
    //     win; all others are detected as replays.
    //
    // Grace-Period note (NOT implemented — mention for awareness):
    //   Frontends that fire N parallel requests on a 401 can legitimately hit this
    //   endpoint simultaneously with the same token.  A grace-period approach would
    //   check (UsedAt < now + N seconds) and return a 409 Conflict telling the
    //   client to back off and retry with the tokens it already received from the
    //   winning request.  This trades a small security window for better UX.
    //   The recommended alternative is a client-side refresh mutex (single in-flight
    //   refresh promise) so the race never reaches the server.
    // ────────────────────────────────────────────────────────────────────────────

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        // ── 1. Validate access token signature (lifetime is intentionally NOT checked) ──
        ClaimsPrincipal principal;
        try
        {
            principal = jwt.GetPrincipalFromExpiredToken(dto.AccessToken);
        }
        catch
        {
            return Unauthorized("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!int.TryParse(userIdClaim, out var claimedUserId))
            return Unauthorized("Malformed token claims.");

        // ── 2. Load the stored refresh token ────────────────────────────────────
        var stored = await refreshTokenRepo.GetByTokenAsync(dto.RefreshToken);

        if (stored is null)
            return Unauthorized("Invalid refresh token.");

        // ── 3. Ownership guard ───────────────────────────────────────────────────
        // The refresh token's UserId MUST match the Sub claim in the access token.
        // A mismatch means someone mixed tokens from different accounts — possible
        // token-confusion attack.  Terminate both users' sessions immediately.
        if (stored.UserId != claimedUserId)
        {
            logger.LogWarning(
                "Token ownership mismatch: access token Sub={ClaimedUserId}, " +
                "refresh token owner={TokenUserId}. Revoking all sessions for both users.",
                claimedUserId, stored.UserId);

            await refreshTokenRepo.RevokeAllForUserAsync(stored.UserId);
            await refreshTokenRepo.RevokeAllForUserAsync(claimedUserId);
            return Unauthorized("Token ownership mismatch. All sessions have been terminated.");
        }

        // ── 4. Classify already-revoked tokens before touching the DB ────────────
        // UsedAt is non-null ONLY when TryConsumeAsync set it (i.e. this token was
        // previously rotated).  A revoked token with UsedAt=null was explicitly
        // logged out via /revoke — that is NOT a replay attack.
        if (stored.IsRevoked)
        {
            if (stored.UsedAt is not null)
            {
                logger.LogWarning(
                    "Replay attack detected for userId={UserId}. " +
                    "Refresh token was consumed at {UsedAt}. Revoking all active sessions.",
                    stored.UserId, stored.UsedAt);

                await refreshTokenRepo.RevokeAllForUserAsync(stored.UserId);
                return Unauthorized("Refresh token reuse detected. All active sessions have been revoked.");
            }

            // Explicitly revoked (logout) — no cascade, just reject.
            return Unauthorized("Refresh token has been revoked.");
        }

        // ── 5. Expiry check ──────────────────────────────────────────────────────
        // An expired but not-yet-revoked token is not a security threat;
        // it just means the user's session aged out naturally.
        if (stored.IsExpired)
            return Unauthorized("Refresh token has expired. Please log in again.");

        // ── 6. Atomic claim — the single point of thread safety ─────────────────
        // Translates to: UPDATE RefreshTokens
        //                SET    IsRevoked=true, UsedAt=now
        //                WHERE  Token=@t AND IsRevoked=false
        //
        // The database row-level lock ensures exactly one concurrent request
        // succeeds.  Any other request racing on the same token receives affected=0.
        var claimed = await refreshTokenRepo.TryConsumeAsync(dto.RefreshToken);

        if (!claimed)
        {
            // Another request won the race on this token.
            // Treat as a potential replay: revoke all sessions for this user.
            //
            // Grace-period alternative: if (DateTime.UtcNow - stored.UsedAt < GracePeriod)
            //   return Conflict("Duplicate request detected — use tokens from the first response.");
            logger.LogWarning(
                "Concurrent refresh detected for userId={UserId}. " +
                "Another request already consumed this token. Revoking all active sessions.",
                stored.UserId);

            await refreshTokenRepo.RevokeAllForUserAsync(stored.UserId);
            return Unauthorized("Concurrent token use detected. All active sessions have been terminated.");
        }

        // ── 7. Issue new token pair ──────────────────────────────────────────────
        var user = await userRepo.GetByIdAsync(stored.UserId);
        if (user is null)
            return Unauthorized("User account no longer exists.");

        var newAccessToken  = jwt.GenerateToken(user);
        var newRefreshToken = jwt.GenerateRefreshToken(user.Id);
        await refreshTokenRepo.AddAsync(newRefreshToken);

        return Ok(new AuthResponseDto
        {
            AccessToken  = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            UserName     = user.UserName,
        });
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Revoke  (explicit logout)
    // ────────────────────────────────────────────────────────────────────────────

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequestDto dto)
    {
        var stored = await refreshTokenRepo.GetByTokenAsync(dto.RefreshToken);
        if (stored is null || !stored.IsActive)
            return BadRequest("Refresh token is invalid or already revoked.");

        await refreshTokenRepo.RevokeAsync(stored);
        return NoContent();
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Me
    // ────────────────────────────────────────────────────────────────────────────

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userName = User.Identity?.Name ?? "Unknown";
        return Ok(new { UserName = userName });
    }
}
