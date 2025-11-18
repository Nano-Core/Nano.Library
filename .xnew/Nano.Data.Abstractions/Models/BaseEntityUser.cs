using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <summary>
/// Base Entity User (abstract).
/// </summary>
public abstract class BaseEntityUser<TIdentity> : BaseEntity<TIdentity>, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    [Include]
    [SwaggerResponseOnly] // BUG: 000: Remove User.IdentityUser? Improve the implementation?
    public virtual IdentityUser<TIdentity> IdentityUser { get; set; }

    /// <inheritdoc />
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}