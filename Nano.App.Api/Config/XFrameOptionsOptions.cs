using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring X-Frame-Options headers.
/// </summary>
public class XFrameOptionsOptions
{
    /// <summary>
    /// Specifies the X-Frame-Options policy header value.
    /// </summary>
    [Required]
    public virtual XFrameOptionsPolicy XFrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;
}