using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Services;

/// <summary>
/// Service interface for Task business logic.
/// This layer sits between Controllers and Repositories, handling business rules and orchestration.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Get all tasks
    /// </summary>
    Task<IEnumerable<TaskResponse>> GetAllAsync();

    /// <summary>
    /// Get a task by ID
    /// </summary>
    Task<TaskResponse?> GetByIdAsync(Guid id);

    /// <summary>
    /// Create a new task
    /// </summary>
    Task<TaskResponse> CreateAsync(CreateTaskRequest request);

    /// <summary>
    /// Update an existing task
    /// </summary>
    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request);

    /// <summary>
    /// Delete a task
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Toggle task completion status
    /// </summary>
    Task<TaskResponse> ToggleCompletionAsync(Guid id);
}
