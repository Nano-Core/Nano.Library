using Nano.Models.Interfaces;

namespace Nano.Models;

/// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
public abstract class BaseEntityIdentity<TIdentity> : BaseEntity, IEntityIdentity<TIdentity>
{
    /// <inheritdoc />
    public TIdentity Id { get; set; }
}