using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// 
/// </summary>
public class XFrameOptionsOptions
{
    /// <summary>
    /// Use XFrame Options Policy Header.
    /// </summary>
    [Required]
    public virtual XFrameOptionsPolicy XFrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;
}