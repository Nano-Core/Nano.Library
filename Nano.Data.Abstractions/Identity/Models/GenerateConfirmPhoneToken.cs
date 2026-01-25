using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GenerateConfirmPhoneToken : GenerateConfirmPhoneToken<Guid>;

/// <summary>
/// Represents a request to generate a phone number confirmation token for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GenerateConfirmPhoneToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user for whom the phone confirmation token is generated.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;
}