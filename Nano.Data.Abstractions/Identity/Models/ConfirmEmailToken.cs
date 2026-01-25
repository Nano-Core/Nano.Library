using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ConfirmEmailToken : ConfirmEmailToken<Guid>;

/// <summary>
/// Represents a request to confirm a user's email using a token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ConfirmEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose email is being confirmed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to confirm the user's email address.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}