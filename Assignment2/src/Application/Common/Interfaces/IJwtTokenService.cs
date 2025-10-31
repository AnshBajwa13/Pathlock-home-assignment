namespace Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate an access token for a user
    /// </summary>
    string GenerateAccessToken(Guid userId, string email);

    /// <summary>
    /// Generate a refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate a token and extract the user ID
    /// </summary>
    Guid? ValidateToken(string token);
}
