namespace ToDoApi.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Set atomically only when consumed via /refresh (never via /revoke).
    // null  → token was explicitly revoked (logout) or is still active.
    // !null → token was consumed in a prior Refresh call; reuse = replay attack.
    public DateTime? UsedAt { get; set; }

    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
