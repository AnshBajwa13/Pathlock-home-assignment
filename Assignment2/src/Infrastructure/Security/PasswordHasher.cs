using Application.Common.Interfaces;

namespace Infrastructure.Security;

/// <summary>
/// Password hashing service using BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hash a plain text password using BCrypt with work factor 12
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    /// <summary>
    /// Verify a password against a BCrypt hash
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
