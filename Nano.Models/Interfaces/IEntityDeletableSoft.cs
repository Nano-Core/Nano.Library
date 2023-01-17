using System.Text.Json.Serialization;

namespace Nano.Models.Interfaces;

/// <summary>
/// Entity Deletable.
/// Implementing entities are deletable.
/// </summary>
public interface IEntityDeletableSoft : IEntityDeletable
{
    /// <summary>
    /// Deleted at.
    /// Indicates whether the entity is deleted or not. Zero means active (true) and anything greater than zero means deleted (false).
    /// THe Unix-based time in miliseconds is stored, in order for allowing unique indexes with soft-deletetion.
    /// NOTE: Only active instances are returned from queries, when filters are enabled (default behavior).
    /// </summary>
    [JsonIgnore]
    long IsDeleted { get; set; }
}