using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for configuring the repository layer and behavior.
/// </summary>
public class RepositoryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether automatic saving of changes in repositories is enabled.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool UseAutoSave { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum depth for query includes.
    /// Defaults to <c>4</c>.
    /// </summary>
    [Required]
    public virtual int QueryIncludeDepth { get; set; } = 4;
}