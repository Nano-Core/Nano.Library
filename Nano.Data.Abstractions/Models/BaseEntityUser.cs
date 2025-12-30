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
    /// <inheritdoc />
    [Include]
    [RequestIgnore]
    public virtual IdentityUserExt<TIdentity> IdentityUserEx { get; set; }
}