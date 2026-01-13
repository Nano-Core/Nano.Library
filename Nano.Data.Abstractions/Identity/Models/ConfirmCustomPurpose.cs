using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ConfirmCustomPurpose : ConfirmCustomPurpose<Guid>;

/// <summary>
/// Confirm Custom Purpose.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ConfirmCustomPurpose<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// Purpose.
    /// </summary>
    [Required]
    public virtual string Purpose { get; set; } = null!;
}