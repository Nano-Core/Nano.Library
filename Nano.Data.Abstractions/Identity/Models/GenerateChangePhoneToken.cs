using System;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Annotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GenerateChangePhoneToken : GenerateChangePhoneToken<Guid>;

/// <summary>
/// Represents a request to generate a change phone number token for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GenerateChangePhoneToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user requesting the phone number change.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The new phone number to be associated with the user.
    /// </summary>
    [Required]
    [MaxLength(20)]
    [InternationalPhone]
    public virtual string NewPhoneNumber { get; set; } = null!;
}