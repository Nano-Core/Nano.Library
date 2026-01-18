using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Config;
using Nano.App.Consts;

namespace Nano.App.Config;

/// <summary>
/// Base App Options (abstract).
/// </summary>
public abstract class BaseAppOptions
{
    internal static string SectionName => "App";

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = AppDefaults.DEFAULT_APP_NAME;

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
    public virtual IDictionary<string, ApiClientOptions> Apis { get; set; } = new Dictionary<string, ApiClientOptions>();
}