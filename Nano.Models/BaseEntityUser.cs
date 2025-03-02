using System;
using Nano.Models.Attributes;
using Nano.Models.Interfaces;

namespace Nano.Models;

/// <summary>
/// Base Entity User (abstract).
/// </summary>
public abstract class BaseEntityUser<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityUser<TIdentity>, IEntityWritable
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public virtual long IsDeleted { get; set; } = 0L;

    /// <inheritdoc />
    public virtual TIdentity IdentityUserId { get; set; }

    /// <inheritdoc />
    [Include]
    [SwaggerResponseOnly]
    public virtual IdentityUserExpanded<TIdentity> IdentityUser { get; set; }
}