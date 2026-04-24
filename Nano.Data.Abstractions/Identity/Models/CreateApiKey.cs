using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to create an API key for a user.
/// </summary>
public class CreateApiKey
{
    /// <summary>
    /// The name of the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Name { get; set; }

    /// <summary>
    /// The optional expiration date of the API key.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }

    /// <summary>
    /// Whether the api key should inherit the roles and claims from it's parent user.
    /// </summary>
    public virtual bool InheritPermissions { get; set; }

    /// <summary>
    /// Roles to be added to the api key.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> Roles { get; set; } = [];

    /// <summary>
    /// Claims to be added to the api key.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}