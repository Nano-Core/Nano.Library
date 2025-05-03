using System.ComponentModel.DataAnnotations;
using Nano.Models.Interfaces;

namespace Nano.Models.Data;

/// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
public abstract class BaseEntityIdentity<TIdentity> : IEntityIdentity<TIdentity>
{
    /// <inheritdoc />
    [Required]
    public TIdentity Id { get; set; }
}