using System;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class RevokeApiKey : RevokeApiKey<Guid>;

/// <summary>
/// Represents a request to revoke an API key.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class RevokeApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The optional timestamp when the API key is revoked.
    /// </summary>
    public virtual DateTimeOffset? RevokeAt { get; set; }
}