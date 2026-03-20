using System;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Represents an entity associated with a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public interface IEntityUser<TIdentity> : IEntityIdentity<TIdentity>, IEntityUpdatable, IEntityDeletable
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the associated identity user.
    /// </summary>
    IdentityUserEx<TIdentity> IdentityUser { get; set; }
}