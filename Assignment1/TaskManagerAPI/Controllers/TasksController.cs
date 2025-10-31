using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

/// <summary>
/// REST API controller for Task operations.
/// This controller is intentionally THIN - all business logic is in TaskService.
/// This demonstrates proper separation of concerns vs fat controllers.
/// </summary>
[ApiController]
[Route("api/v1/tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    /// <returns>List of all tasks</returns>
    /// <response code="200">Returns the list of tasks</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    /// <response code="200">Returns the task</response>
    /// <response code="404">Task not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        
        if (task == null)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return Ok(task);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="request">Task creation request</param>
    /// <returns>Created task</returns>
    /// <response code="201">Task created successfully</response>
    /// <response code="400">Invalid request</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var task = await _taskService.CreateAsync(request);
        
        // Return 201 Created with Location header pointing to the new resource
        return CreatedAtAction(
            nameof(GetById), 
            new { id = task.Id }, 
            task
        );
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated task</returns>
    /// <response code="200">Task updated successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">Task not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var task = await _taskService.UpdateAsync(id, request);
        return Ok(task);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>No content</returns>
    /// <response code="204">Task deleted successfully</response>
    /// <response code="404">Task not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _taskService.DeleteAsync(id);
        
        if (!deleted)
            return NotFound(new { message = $"Task with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Toggle task completion status
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Updated task</returns>
    /// <response code="200">Task toggled successfully</response>
    /// <response code="404">Task not found</response>
    [HttpPatch("{id}/toggle")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleCompletion(Guid id)
    {
        var task = await _taskService.ToggleCompletionAsync(id);
        return Ok(task);
    }
}
