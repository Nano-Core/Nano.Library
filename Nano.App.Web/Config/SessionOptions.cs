using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Session Options.
/// </summary>
public class SessionOptions
{
    /// <summary>
    /// Timeout.
    /// The session timeout.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(20);
}