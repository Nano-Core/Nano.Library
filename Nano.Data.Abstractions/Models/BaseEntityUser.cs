using System;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Abstractions.Models;

/// <summary>
/// Base Entity User (abstract).
/// </summary>
public abstract class BaseEntityUser<TIdentity> : BaseEntity<TIdentity>, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    // BUG: Shouldn't we have IdentityUserExId
    // BUG: Reanme IdentityUser ?? I think so because it becomes the FK in db

    /// <inheritdoc />
    [Include]
    [RequestIgnore]
    public virtual IdentityUserEx<TIdentity> IdentityUserEx { get; set; } = null!;
}