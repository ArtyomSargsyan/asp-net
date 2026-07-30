using ToDoApi.Models;

namespace ToDoApi.Repositories.RefreshTokens;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);

    // Atomically marks the token as consumed (IsRevoked=true, UsedAt=now).
    // Returns true if the claim succeeded, false if already revoked (race condition / replay).
    Task<bool> TryConsumeAsync(string token);

    Task RevokeAsync(RefreshToken refreshToken);
    Task RevokeAllForUserAsync(int userId);
}
