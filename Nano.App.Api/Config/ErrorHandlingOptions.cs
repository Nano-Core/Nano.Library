using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Errror handling options.
/// </summary>
public class ErrorHandlingOptions
{
    /// <summary>
    /// Expose detailed internal server errors.
    /// </summary>
    [Required]
    public virtual bool ExposeErrors { get; set; } = false;
}