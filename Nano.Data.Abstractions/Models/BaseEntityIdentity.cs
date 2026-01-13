using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
public abstract class BaseEntityIdentity<TIdentity> : IEntityIdentity<TIdentity>
{
    /// <inheritdoc />
    [Required]
    public TIdentity Id { get; set; } = default!;
}