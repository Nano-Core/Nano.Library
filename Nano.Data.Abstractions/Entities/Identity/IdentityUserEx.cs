using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Entities.Identity;

/// <summary>
/// Extended identity user with additional properties.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityUserEx<TIdentity> : IdentityUser<TIdentity>, IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}