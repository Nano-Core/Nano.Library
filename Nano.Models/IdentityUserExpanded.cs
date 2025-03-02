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

    /// <summary>
    /// New Email.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmail { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [Phone]
    [MaxLength(40)]
    public virtual string NewPhoneNumber { get; set; }
}