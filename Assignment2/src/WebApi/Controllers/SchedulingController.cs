using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Scheduling.Commands.GenerateSchedule;

namespace ProjectManager.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/scheduling")]
public class SchedulingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SchedulingController> _logger;

    public SchedulingController(IMediator mediator, ILogger<SchedulingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("generate/{projectId}")]
    public async Task<IActionResult> GenerateSchedule(Guid projectId)
    {
        _logger.LogInformation("Generating schedule for project: {ProjectId}", projectId);

        var command = new GenerateScheduleCommand { ProjectId = projectId };
        var result = await _mediator.Send(command);

        _logger.LogInformation("Schedule generated successfully for project: {ProjectId}", projectId);
        
        return Ok(result);
    }
}
