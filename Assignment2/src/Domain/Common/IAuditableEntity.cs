namespace Domain.Common;

/// <summary>
/// Interface for entities that track audit information
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// When the entity was created
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    Guid CreatedBy { get; set; }

    /// <summary>
    /// When the entity was last updated
    /// </summary>
    DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User who last updated the entity
    /// </summary>
    Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// When the entity was deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }
}
