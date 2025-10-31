using Application.Auth.DTOs;
using Application.Common.Models;
using MediatR;

namespace Application.Auth.Commands.Register;

/// <summary>
/// Command to register a new user
/// </summary>
public class RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}
