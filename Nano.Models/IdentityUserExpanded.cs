using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nano.Models;

/// <inheritdoc />
public class IdentityUserExpanded<TKey> : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Is Active.
    /// </summary>
    [Required]
    [DefaultValue(true)]
    public virtual bool IsActive { get; set; } = true;
}