using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Project entity - represents a project that contains tasks
/// </summary>
public class Project : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Project title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Project description (optional)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// ID of the user who owns this project
    /// </summary>
    public Guid UserId { get; set; }

    // IAuditableEntity implementation

    /// <summary>
    /// When the project was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the project
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// When the project was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who last updated the project
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// When the project was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// User who owns this project
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Tasks in this project
    /// </summary>
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
