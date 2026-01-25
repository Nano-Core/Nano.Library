using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GenerateChangeEmailToken : GenerateChangeEmailToken<Guid>;

/// <summary>
/// Represents a request to generate a change email token for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GenerateChangeEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user requesting the email change.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The new email address to be associated with the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmailAddress { get; set; } = null!;
}