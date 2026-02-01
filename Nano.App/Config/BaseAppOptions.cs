using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Config;

namespace Nano.App.Config;

/// <summary>
/// Base application configuration options.
/// Serves as the root options object for Nano-based applications.
/// </summary>
public abstract class BaseAppOptions
{
    internal static string SectionName => "App";

    /// <summary>
    /// Application version identifier.
    /// Used for diagnostics, documentation, and versioning metadata.
    /// </summary>
    [Required]
    public virtual string Version { get; set; } = "1.0.0.0";

    /// <summary>
    /// Localization configuration options.
    /// </summary>
    public virtual LocalizationOptions? Localization { get; set; }

    /// <summary>
    /// Named Nano API client configurations available to the application.
    /// </summary>
    [Required]
    public virtual IDictionary<string, ApiClientOptions> Apis { get; set; } = new Dictionary<string, ApiClientOptions>();
}