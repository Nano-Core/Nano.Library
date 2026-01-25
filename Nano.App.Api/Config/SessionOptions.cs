using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring session behavior.
/// </summary>
public class SessionOptions
{
    /// <summary>
    /// Session timeout duration.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(20);
}