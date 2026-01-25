using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GenerateConfirmEmailToken : GenerateConfirmEmailToken<Guid>;

/// <summary>
/// Represents a request to generate an email confirmation token for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GenerateConfirmEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user for whom the email confirmation token is generated.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;
}