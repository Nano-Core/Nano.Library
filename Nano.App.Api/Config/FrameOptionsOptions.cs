using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring X-Frame-Options headers.
/// </summary>
public class FrameOptionsOptions
{
    /// <summary>
    /// Specifies the X-Frame-Options policy header value.
    /// </summary>
    [Required]
    public virtual XFrameOptionsPolicy FrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;
}