using System;
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
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// The optional expiration date of the API key.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}