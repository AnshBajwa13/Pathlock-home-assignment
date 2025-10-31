namespace Domain.Common;

/// <summary>
/// Base class for all entities with common properties
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; }
}
