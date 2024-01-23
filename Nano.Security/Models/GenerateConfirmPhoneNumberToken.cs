using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Generate Confirm Phone Number Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateConfirmPhoneNumberToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }
}