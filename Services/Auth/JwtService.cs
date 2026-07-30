using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDoApi.Models;

namespace ToDoApi.Services.Auth;

public sealed class JwtService(IConfiguration config, TimeProvider? timeProvider = null)
{
    private const int MinKeyBytes = 32;

    private readonly string       _key                = ValidateKey(config["Jwt:Key"]);
    private readonly string       _issuer             = config["Jwt:Issuer"]          ?? string.Empty;
    private readonly string       _audience           = config["Jwt:Audience"]        ?? string.Empty;
    private readonly int          _expiryHours        = int.TryParse(config["Jwt:ExpireHours"],       out var h) ? h : 2;
    private readonly int          _refreshExpiryDays  = int.TryParse(config["Jwt:RefreshExpireDays"], out var d) ? d : 7;
    private readonly TimeProvider _time               = timeProvider ?? TimeProvider.System;

    public string GenerateToken(User user)
    {
        // C# 12 collection expression instead of `new[]`
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Email,      user.Email),
            new("Role",                             user.Role ?? "User"),
        ];

        var signingKey  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // TimeProvider.GetUtcNow() returns DateTimeOffset — no implicit UTC drift.
        var now = _time.GetUtcNow().UtcDateTime;

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            now.AddHours(_expiryHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(int userId)
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);

        return new RefreshToken
        {
            Token     = Convert.ToBase64String(bytes),
            UserId    = userId,
            ExpiresAt = _time.GetUtcNow().UtcDateTime.AddDays(_refreshExpiryDays),
            CreatedAt = _time.GetUtcNow().UtcDateTime,
        };
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = false, // intentionally skipped — token may be expired
            ValidateIssuerSigningKey = true,
            ValidIssuer              = _issuer,
            ValidAudience            = _audience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
        };

        var handler = new JwtSecurityTokenHandler();

        // Mirrors the MapInboundClaims = false setting in IdentityServiceExtensions.
        // Without this, "sub" is silently remapped to ClaimTypes.NameIdentifier,
        // causing FindFirst(JwtRegisteredClaimNames.Sub) to return null.
        handler.InboundClaimTypeMap.Clear();

        var principal = handler.ValidateToken(token, validationParams, out var validated);

        if (validated is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            throw new SecurityTokenException("Invalid token algorithm.");

        return principal;
    }

    // Static helper: can be called from a field initializer (no `this` needed).
    private static string ValidateKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException(
                "JWT key is missing or empty. Provide 'Jwt:Key' via environment variables or user secrets — never in appsettings.json.");

        if (Encoding.UTF8.GetByteCount(key) < MinKeyBytes)
            throw new InvalidOperationException(
                $"JWT key is too short. HMAC-SHA256 requires at least {MinKeyBytes} bytes ({MinKeyBytes * 8} bits).");

        return key;
    }
}
