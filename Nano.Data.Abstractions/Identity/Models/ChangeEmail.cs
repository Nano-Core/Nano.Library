using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class ChangeEmail : ChangeEmail<Guid>;

/// <summary>
/// Represents a request to change a user's email address.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class ChangeEmail<TIdentity>
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
}