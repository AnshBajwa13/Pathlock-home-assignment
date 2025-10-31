using TaskManagerAPI.Models.Domain;

namespace TaskManagerAPI.Repositories;

/// <summary>
/// Repository interface for Task operations.
/// This abstraction allows easy swapping of storage implementations (in-memory, database, etc.)
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Get all tasks
    /// </summary>
    Task<IEnumerable<TaskItem>> GetAllAsync();

    /// <summary>
    /// Get a task by ID
    /// </summary>
    Task<TaskItem?> GetByIdAsync(Guid id);

    /// <summary>
    /// Add a new task
    /// </summary>
    Task AddAsync(TaskItem task);

    /// <summary>
    /// Update an existing task
    /// </summary>
    Task UpdateAsync(TaskItem task);

    /// <summary>
    /// Delete a task by ID
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if a task exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
