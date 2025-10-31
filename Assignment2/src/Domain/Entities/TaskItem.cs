using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// TaskItem entity - represents a task within a project
/// </summary>
public class TaskItem : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Task title (required)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the task is completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// When the task was completed (if completed)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Due date for the task (optional)
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Estimated hours to complete this task (for scheduling optimization)
    /// Optional field added for Assignment 3 enhancement
    /// Supports decimal values (e.g., 0.5, 2.5) for better precision
    /// </summary>
    public decimal? EstimatedHours { get; set; }

    /// <summary>
    /// ID of the project this task belongs to
    /// </summary>
    public Guid ProjectId { get; set; }

    // IAuditableEntity implementation

    /// <summary>
    /// When the task was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the task
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// When the task was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who last updated the task
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// When the task was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// Project this task belongs to
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Tasks that this task depends on (prerequisites that must be completed first)
    /// Added for Assignment 3 enhancement - enables task scheduling and dependency analysis
    /// </summary>
    public ICollection<TaskDependency> DependsOn { get; set; } = new List<TaskDependency>();

    /// <summary>
    /// Tasks that depend on this task (tasks blocked by this task)
    /// Added for Assignment 3 enhancement - enables reverse dependency traversal
    /// </summary>
    public ICollection<TaskDependency> RequiredBy { get; set; } = new List<TaskDependency>();

    // Domain methods

    /// <summary>
    /// Mark the task as completed
    /// </summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark the task as incomplete
    /// </summary>
    public void MarkAsIncomplete()
    {
        IsCompleted = false;
        CompletedAt = null;
    }

    /// <summary>
    /// Toggle completion status
    /// </summary>
    public void ToggleCompletion()
    {
        if (IsCompleted)
        {
            MarkAsIncomplete();
        }
        else
        {
            MarkAsCompleted();
        }
    }
}
