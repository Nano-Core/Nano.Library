using System.ComponentModel.DataAnnotations;

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
}