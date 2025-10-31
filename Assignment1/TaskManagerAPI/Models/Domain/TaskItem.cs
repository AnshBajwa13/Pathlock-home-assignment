namespace TaskManagerAPI.Models.Domain;

/// <summary>
/// Rich domain model for a Task Item.
/// Uses private setters for encapsulation and behavior methods instead of public setters.
/// This demonstrates Domain-Driven Design principles even in a simple CRUD app.
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Unique identifier for the task
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Description of the task
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Whether the task is completed
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// When the task was created (UTC)
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the task was completed (UTC), null if not completed
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    // Private parameterless constructor for serialization/EF Core if needed later
    private TaskItem() { }

    /// <summary>
    /// Create a new task with a description
    /// </summary>
    /// <param name="description">Task description</param>
    /// <exception cref="ArgumentException">If description is null or whitespace</exception>
    public TaskItem(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Id = Guid.NewGuid();
        Description = description;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    /// <summary>
    /// Update the task description
    /// </summary>
    /// <param name="description">New description</param>
    /// <exception cref="ArgumentException">If description is null or whitespace</exception>
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Description = description;
    }

    /// <summary>
    /// Mark the task as completed
    /// </summary>
    public void MarkAsCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Mark the task as incomplete
    /// </summary>
    public void MarkAsIncomplete()
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            CompletedAt = null;
        }
    }

    /// <summary>
    /// Toggle completion status
    /// </summary>
    public void ToggleCompletion()
    {
        if (IsCompleted)
            MarkAsIncomplete();
        else
            MarkAsCompleted();
    }
}
