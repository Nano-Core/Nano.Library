using System;
using Nano.Data.Abstractions.Entities.Identity;

namespace Nano.Data.Abstractions.Entities.Abstractions;

/// <summary>
/// Represents an entity associated with a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public interface IEntityUser<TIdentity> : IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the associated identity user.
    /// </summary>
    IdentityUserEx<TIdentity> IdentityUser { get; set; }
}