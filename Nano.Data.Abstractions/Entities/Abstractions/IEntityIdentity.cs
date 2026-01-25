using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Entities.Abstractions;

/// <summary>
/// Represents an entity with a unique identity.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity's identity key.</typeparam>
public interface IEntityIdentity<TIdentity> : IEntityAuditable
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Required]
    TIdentity Id { get; set; }
}