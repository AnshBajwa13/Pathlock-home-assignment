using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Root endpoint - API health check
    /// </summary>
    [HttpGet("/")]
    public IActionResult Index()
    {
        return Ok(new
        {
            service = "Mini Project Manager API",
            status = "running",
            version = "1.0.0",
            timestamp = DateTime.UtcNow,
            endpoints = new
            {
                swagger = "/swagger",
                auth = "/api/auth",
                projects = "/api/projects",
                tasks = "/api/tasks",
                scheduling = "/api/scheduling"
            }
        });
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow
        });
    }
}
