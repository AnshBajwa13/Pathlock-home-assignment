namespace Application.Common.Interfaces;

/// <summary>
/// Interface for retrieving the current authenticated user
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get the current user's ID
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Get the current user's email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
