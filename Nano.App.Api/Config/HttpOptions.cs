using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for http.
/// </summary>
public class HttpOptions
{
    /// <summary>
    /// List of ports for HTTP.'
    /// </summary>
    [Required]
    public virtual int[] Ports { get; set; } = [];

    /// <summary>
    /// Enforce HTTPS redirect for all requests.
    /// If no ports are configured for https, this option is ignored.
    /// </summary>
    [Required]
    public virtual bool UseHttpsRedirection { get; set; } = false;
}