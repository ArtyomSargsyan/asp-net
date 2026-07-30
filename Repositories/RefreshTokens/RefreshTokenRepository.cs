using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;

namespace ToDoApi.Repositories.RefreshTokens;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == token);

    public async Task AddAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();
    }

    // Single atomic UPDATE … WHERE Token = @t AND IsRevoked = false.
    // The database serializes concurrent writes to the same row, so exactly one
    // caller will receive affected = 1; all racing duplicates receive affected = 0.
    public async Task<bool> TryConsumeAsync(string token)
    {
        var now = DateTime.UtcNow;
        var affected = await context.RefreshTokens
            .Where(r => r.Token == token && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.IsRevoked, true)
                .SetProperty(r => r.UsedAt, now));

        return affected > 0;
    }

    // Used only by /revoke (explicit logout). Does NOT set UsedAt, so
    // the controller can distinguish logout from replay on a reuse attempt.
    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens
            .Where(r => r.Id == refreshToken.Id && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true));
    }

    public async Task RevokeAllForUserAsync(int userId)
    {
        await context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true));
    }
}
