using Application.Auth.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for refreshing access token
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find refresh token
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null)
        {
            return Result<TokenResponse>.Failure("Invalid refresh token");
        }

        // Check if token is active (not expired or revoked)
        if (!refreshToken.IsActive)
        {
            return Result<TokenResponse>.Failure("Refresh token is no longer valid");
        }

        // Revoke old refresh token
        refreshToken.Revoke();

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateAccessToken(refreshToken.UserId, refreshToken.User.Email);
        var newRefreshTokenString = _jwtTokenService.GenerateRefreshToken();

        // Create new refresh token entity
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenString,
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        // Return token response
        var response = new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenString,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(15) // Access token expiry
        };

        return Result<TokenResponse>.Success(response);
    }
}
