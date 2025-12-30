using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Config;

namespace Nano.App.Config;

/// <summary>
/// Base App Options (abstract).
/// </summary>
public abstract class BaseAppOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public static string SectionName => "App";

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = "Application";

    /// <summary>
    /// Entry Point.
    /// </summary>
    [Required]
    public virtual string EntryPoint { get; set; }

    /// <summary>
    /// Version.
    /// </summary>
    [Required]
    public virtual string Version { get; set; } = "1.0.0.0";

    /// <summary>
    /// Cultures.
    /// </summary>
    [Required]
    public virtual CultureOptions Cultures { get; set; } = new();

    /// <summary>
    /// Apis.
    /// </summary>
    [Required]
    public virtual IDictionary<string, ApiOptions> Apis { get; set; } = new Dictionary<string, ApiOptions>();
}