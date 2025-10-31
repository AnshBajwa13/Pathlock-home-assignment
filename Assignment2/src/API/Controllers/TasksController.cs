using Application.Tasks.Commands.CreateTask;
using Application.Tasks.Commands.DeleteTask;
using Application.Tasks.Commands.ToggleTaskCompletion;
using Application.Tasks.Commands.UpdateTask;
using Application.Tasks.Queries.GetTaskById;
using Application.Tasks.Queries.GetTasksByProject;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Tasks management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get tasks by project ID
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTasksByProject(Guid projectId, [FromQuery] bool? isCompleted)
    {
        var query = new GetTasksByProjectQuery { ProjectId = projectId, IsCompleted = isCompleted };
        var result = await _mediator.Send(query);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var query = new GetTaskByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return CreatedAtAction(nameof(GetTaskById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Toggle task completion status
    /// </summary>
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTaskCompletion(Guid id)
    {
        var command = new ToggleTaskCompletionCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Delete a task (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var command = new DeleteTaskCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = result.Errors });
        }

        return NoContent();
    }
}
