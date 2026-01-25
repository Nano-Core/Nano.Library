using System;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Entities.Identity;

namespace Nano.Data.Abstractions.Entities;

/// <summary>
/// Base class for entities associated with a user.
/// Implements <see cref="IEntityUser{TIdentity}"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public abstract class BaseEntityUser<TIdentity> : BaseEntity<TIdentity>, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The associated identity user.
    /// Marked with <see cref="IncludeAttribute"/> for EF include and <see cref="RequestIgnoreAttribute"/> to ignore during requests.
    /// </summary>
    [Include]
    [RequestIgnore]
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;
}