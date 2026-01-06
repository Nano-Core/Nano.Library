using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity User Ext.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class IdentityUserEx<TIdentity> : IdentityUser<TIdentity>, IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Is Active.
    /// </summary>
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}