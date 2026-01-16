using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Annotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity Api Key Created.
/// </summary>
public class IdentityApiKeyCreated<TIdentity> : IdentityApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Api Key.
    /// </summary>
    [Required]
    [Include]
    public virtual IdentityApiKey<TIdentity> IdentityApiKey { get; set; } = null!;

    /// <summary>
    /// Unencrypted Hash.
    /// </summary>
    [Required]
    public virtual string UnencryptedHash { get; set; } = null!;
}