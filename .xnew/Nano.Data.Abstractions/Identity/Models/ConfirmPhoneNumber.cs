using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class ConfirmPhoneNumber : ConfirmPhoneNumber<Guid>;

/// <summary>
/// Confirm Phone Number.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ConfirmPhoneNumber<TIdentity>
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
}