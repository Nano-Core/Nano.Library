using Nano.Common.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GenerateCustomPurposeToken : GenerateCustomPurposeToken<Guid>;

/// <summary>
/// Represents a request to generate a custom-purpose token for a user.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GenerateCustomPurposeToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user for whom the token is generated.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// The purpose for which the token is generated.
    /// </summary>
    [MaxLength(256)]
    public virtual string Purpose { get; set; } = null!;
}