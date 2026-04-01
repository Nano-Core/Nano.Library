using System;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to revoke an API key.
/// </summary>
public class RevokeApiKey
{
    /// <summary>
    /// The optional timestamp when the API key is revoked.
    /// </summary>
    public virtual DateTimeOffset? RevokeAt { get; set; }
}