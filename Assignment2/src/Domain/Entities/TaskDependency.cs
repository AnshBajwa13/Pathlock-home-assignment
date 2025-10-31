namespace Domain.Entities;

/// <summary>
/// Represents a dependency relationship between two tasks
/// A task cannot start until all its dependencies are completed
/// </summary>
public class TaskDependency
{
    /// <summary>
    /// The task that has the dependency (the dependent/blocked task)
    /// </summary>
    public Guid TaskId { get; set; }
    
    /// <summary>
    /// Navigation property to the dependent task
    /// </summary>
    public TaskItem Task { get; set; } = null!;

    /// <summary>
    /// The task that must be completed first (the prerequisite/blocking task)
    /// </summary>
    public Guid DependsOnTaskId { get; set; }
    
    /// <summary>
    /// Navigation property to the prerequisite task
    /// </summary>
    public TaskItem DependsOnTask { get; set; } = null!;

    /// <summary>
    /// When this dependency relationship was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
