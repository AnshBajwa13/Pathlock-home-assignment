using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.DeleteProject;
using Application.Projects.Commands.UpdateProject;
using Application.Projects.Queries.GetProjectById;
using Application.Projects.Queries.GetProjects;
using Application.Tasks.Commands.CreateTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Projects management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IMediator mediator, ILogger<ProjectsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all projects for the current user with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting projects - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        var query = new GetProjectsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to get projects. Errors: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get a project by ID with full details
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        _logger.LogInformation("Getting project by ID: {ProjectId}", id);

        var query = new GetProjectByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to get project {ProjectId}. Errors: {Errors}",
                id, string.Join(", ", result.Errors));
            return NotFound(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        _logger.LogInformation("Creating project: {Title}", command.Title);

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to create project. Errors: {Errors}",
                string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation("Project created successfully: {ProjectId}", result.Data?.Id);
        return CreatedAtAction(nameof(GetProjectById), new { id = result.Data?.Id }, result.Data);
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { errors = new[] { "URL ID and body ID do not match" } });
        }

        _logger.LogInformation("Updating project: {ProjectId}", id);

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to update project {ProjectId}. Errors: {Errors}",
                id, string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation("Project updated successfully: {ProjectId}", id);
        return Ok(result.Data);
    }

    /// <summary>
    /// Delete a project (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        _logger.LogInformation("Deleting project: {ProjectId}", id);

        var command = new DeleteProjectCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to delete project {ProjectId}. Errors: {Errors}",
                id, string.Join(", ", result.Errors));
            return NotFound(new { errors = result.Errors });
        }

        _logger.LogInformation("Project deleted successfully: {ProjectId}", id);
        return NoContent();
    }

    /// <summary>
    /// Create a new task for a project
    /// </summary>
    [HttpPost("{projectId}/tasks")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskCommand command)
    {
        // Ensure projectId from route matches command
        if (projectId != command.ProjectId)
        {
            return BadRequest(new { errors = new[] { "Project ID mismatch" } });
        }

        _logger.LogInformation("Creating task for project: {ProjectId}", projectId);

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to create task. Errors: {Errors}",
                string.Join(", ", result.Errors));
            return BadRequest(new { errors = result.Errors });
        }

        _logger.LogInformation("Task created successfully: {TaskId}", result.Data?.Id);
        return Created($"/api/tasks/{result.Data?.Id}", result.Data);
    }
}
