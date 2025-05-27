using Nano.Models.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class GenerateChangePhoneToken : GenerateChangePhoneToken<Guid>;

/// <summary>
/// Generate Change Phone Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateChangePhoneToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [Required]
    [InternationalPhone]
    public virtual string NewPhoneNumber { get; set; }
}