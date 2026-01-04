namespace ProManage360.Domain.Entities;

using ProManage360.Domain.Common;

/// <summary>
/// RefreshToken entity - JWT refresh tokens
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid RefreshTokenId { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }

    // Computed properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}