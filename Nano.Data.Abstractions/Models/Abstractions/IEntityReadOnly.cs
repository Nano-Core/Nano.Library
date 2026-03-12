using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Represents a read-only entity with a unique identity.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public interface IEntityReadOnly<TIdentity> : IEntityIdentity<TIdentity>
{
    /// <summary>
    /// Soft delete flag.
    /// Zero means active; any value greater than zero indicates the entity is deleted.
    /// </summary>
    [Required]
    long IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    [Required]
    DateTimeOffset CreatedAt { get; set; }
}