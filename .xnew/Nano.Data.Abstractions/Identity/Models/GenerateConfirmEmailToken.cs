using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class GenerateConfirmEmailToken : GenerateConfirmEmailToken<Guid>;

/// <summary>
/// Generate Confirm Email Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateConfirmEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }
}