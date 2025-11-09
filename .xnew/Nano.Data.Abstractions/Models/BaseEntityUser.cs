using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Attributes;
using Nano.Models.Interfaces;

namespace Nano.Models.Data;

/// <summary>
/// Base Entity User (abstract).
/// </summary>
public abstract class BaseEntityUser<TIdentity> : BaseEntity<TIdentity>, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    [Include]
    [SwaggerResponseOnly]
    public virtual IdentityUser<TIdentity> IdentityUser { get; set; }

    /// <inheritdoc />
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}