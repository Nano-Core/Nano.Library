using System;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityUser : BaseEntityUser<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityUser"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityUser()
    {
        this.Id = Guid.NewGuid();
    }
}

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