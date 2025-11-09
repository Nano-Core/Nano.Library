using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class GenerateCustomPurposeToken : GenerateCustomPurposeToken<Guid>;

/// <summary>
/// Generate Custom Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateCustomPurposeToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Purpose.
    /// </summary>
    public virtual string Purpose { get; set; }
}