using Application.Scheduling.Commands.GenerateSchedule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for task scheduling and work planning
/// </summary>
[ApiController]
[Route("api/scheduling")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(IMediator mediator, ILogger<ScheduleController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generate an optimized schedule for project tasks
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>Recommended task order with critical path analysis</returns>
    /// <response code="200">Returns the generated schedule</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Project not found or access denied</response>
    [HttpPost("generate/{projectId}")]
    [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScheduleDto>> GenerateSchedule(Guid projectId)
    {
        _logger.LogInformation("Generating schedule for project: {ProjectId}", projectId);
        
        var command = new GenerateScheduleCommand { ProjectId = projectId };
        var result = await _mediator.Send(command);
        
        _logger.LogInformation("Schedule generated successfully for project: {ProjectId}", projectId);
        
        return Ok(result);
    }
}
