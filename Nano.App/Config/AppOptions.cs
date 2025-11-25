using System.ComponentModel.DataAnnotations;

namespace Nano.App.Config;

// BUG: OPTIONS: Use IOptionsMonitor<>, maybe that allows for reload on changes.
// BUG: OPTIONS: Add required etc validation annotations on options.
// BUG: OPTIONS: Check usages of nested and whether they can be null.

// BUG: Search using AppOptions and Use IOptionsMonitor<>, or cached things like app name, etc

/// <summary>
/// App Options.
/// </summary>
public class AppOptions
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
    /// Description.
    /// </summary>
    [Required]
    public virtual string Description { get; set; }

    /// <summary>
    /// Terms Of Service.
    /// </summary>
    [Required]
    public virtual string TermsOfService { get; set; }

    /// <summary>
    /// Default Time Zone.
    /// </summary>
    [Required]
    public virtual string DefaultTimeZone { get; set; } = "UTC";

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
    /// Culture Options (nested class).
    /// </summary>
    public class CultureOptions
    {
        /// <summary>
        /// Default.
        /// </summary>
        [Required]
        public virtual string Default { get; set; } = "en-US";

        /// <summary>
        /// Supported.
        /// </summary>
        [Required]
        public virtual string[] Supported { get; set; } = [];
    }
}