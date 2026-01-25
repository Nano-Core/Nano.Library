using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ConfirmCustomPurpose : ConfirmCustomPurpose<Guid>;

/// <summary>
/// Represents a request to confirm a custom purpose.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ConfirmCustomPurpose<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose custom purpose is being confirmed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to confirm the custom purpose.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The name of the custom purpose being confirmed.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Purpose { get; set; } = null!;
}