using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// RefreshToken entity - represents a JWT refresh token for a user
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// The actual refresh token string
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// When the refresh token expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// When the token was revoked (if revoked)
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// When the token was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User this refresh token belongs to
    /// </summary>
    public Guid UserId { get; set; }

    // Navigation properties

    /// <summary>
    /// User who owns this refresh token
    /// </summary>
    public User User { get; set; } = null!;

    // Helper properties

    /// <summary>
    /// Check if the token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Check if the token is active (not expired and not revoked)
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    // Domain methods

    /// <summary>
    /// Revoke this refresh token
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
