using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangeEmailToken : ChangeEmailToken<Guid>;

/// <summary>
/// Represents a request to change a user's email address using a confirmation token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ChangeEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user whose email is being changed.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The token used to authorize the email change.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The new email address to assign to the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmailAddress { get; set; } = null!;
}