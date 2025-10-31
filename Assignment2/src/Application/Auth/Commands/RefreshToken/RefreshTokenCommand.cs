using Application.Auth.DTOs;
using Application.Common.Models;
using MediatR;

namespace Application.Auth.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token using a refresh token
/// </summary>
public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
