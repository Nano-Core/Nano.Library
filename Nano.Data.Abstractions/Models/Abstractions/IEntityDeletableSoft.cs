namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Represents an entity that supports soft deletion.
/// </summary>
public interface IEntityDeletableSoft : IEntityDeletable
{
    /// <summary>
    /// Gets or sets the deletion flag.
    /// Zero means active; any value greater than zero indicates the entity is deleted.
    /// Stores Unix-based time in milliseconds to allow unique indexes with soft deletion.
    /// Only active instances are returned from queries when filters are enabled (default behavior).
    /// </summary>
    long IsDeleted { get; set; }
}