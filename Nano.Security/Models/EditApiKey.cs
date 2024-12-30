using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Edit Api Key.
/// </summary>
public class EditApiKey
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}