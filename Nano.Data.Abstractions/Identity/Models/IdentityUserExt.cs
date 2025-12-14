using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Identity User Ext.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class IdentityUserExt<TIdentity> : IdentityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Is Active.
    /// </summary>
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}