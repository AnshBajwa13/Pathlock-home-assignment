using Application.Auth.Commands.Login;
using Application.Auth.Commands.RefreshToken;
using Application.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Authentication endpoints for user registration, login, and token refresh
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        _logger.LogInformation("User registration attempt for email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration failed for email: {Email}. Errors: {Errors}",
                command.Email, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation("User registered successfully: {UserId}", result.Data?.UserId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("User login attempt for email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Login failed for email: {Email}. Errors: {Errors}",
                command.Email, string.Join(", ", result.Errors));
            return Unauthorized(new { errors = result.Errors });
        }

        _logger.LogInformation("User logged in successfully: {UserId}", result.Data?.UserId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        _logger.LogInformation("Token refresh attempt");

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Token refresh failed. Errors: {Errors}",
                string.Join(", ", result.Errors));
            return Unauthorized(new { errors = result.Errors });
        }

        _logger.LogInformation("Token refreshed successfully");
        return Ok(result.Data);
    }
}
