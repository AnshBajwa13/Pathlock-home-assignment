namespace Application.Common.Interfaces;

/// <summary>
/// Interface for password hashing operations
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plain text password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    bool VerifyPassword(string password, string hash);
}
