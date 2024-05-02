using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Custom Purpose Token
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class CustomPurposeToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// Purpose.
    /// </summary>
    [Required]
    public virtual string Purpose { get; set; }
}