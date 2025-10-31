namespace TaskManagerAPI.Models.DTOs;

/// <summary>
/// Request model for creating a new task
/// </summary>
public record CreateTaskRequest
{
    /// <summary>
    /// Description of the task to create
    /// </summary>
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Request model for updating an existing task
/// </summary>
public record UpdateTaskRequest
{
    /// <summary>
    /// Updated description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Updated completion status
    /// </summary>
    public bool IsCompleted { get; init; }
}

/// <summary>
/// Response model for task data
/// </summary>
public record TaskResponse
{
    /// <summary>
    /// Task ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Task description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Whether the task is completed
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// When the task was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// When the task was completed (UTC), null if not completed
    /// </summary>
    public DateTime? CompletedAt { get; init; }
}
