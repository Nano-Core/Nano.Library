namespace Nano.App.Config;

// BUG: OPTIONS: Use IOptionsMonitor<>, maybe that allows for reload on changes.
// BUG: OPTIONS: Add required etc validation annotations on options.
// BUG: OPTIONS: Check usages of nested and whether they can be null.

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
    public virtual string Name { get; set; } = "Application";

    /// <summary>
    /// Entry Point.
    /// </summary>
    public virtual string EntryPoint { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    public virtual string Description { get; set; }

    /// <summary>
    /// Terms Of Service.
    /// </summary>
    public virtual string TermsOfService { get; set; }

    /// <summary>
    /// Default Time Zone.
    /// </summary>
    public virtual string DefaultTimeZone { get; set; } = "UTC";

    /// <summary>
    /// Version.
    /// </summary>
    public virtual string Version { get; set; } = "1.0.0.0";

    /// <summary>
    /// Cultures.
    /// </summary>
    public virtual CultureOptions Cultures { get; set; } = new();

    /// <summary>
    /// Culture Options (nested class).
    /// </summary>
    public class CultureOptions
    {
        /// <summary>
        /// Default.
        /// </summary>
        public virtual string Default { get; set; } = "en-US";

        /// <summary>
        /// Supported.
        /// </summary>
        public virtual string[] Supported { get; set; } = [];
    }
}