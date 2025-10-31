using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint for monitoring
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "Mini Project Manager API",
            status = "healthy",
            version = "1.0.0",
            timestamp = DateTime.UtcNow,
            endpoints = new
            {
                documentation = "/",
                auth = "/api/auth",
                projects = "/api/projects",
                tasks = "/api/tasks",
                scheduling = "/api/scheduling",
                health = "/api/health"
            }
        });
    }
}
