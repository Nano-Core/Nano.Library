using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// 
/// </summary>
public class XXssProtectionOptions
{
    /// <summary>
    /// 
    /// </summary>
    public virtual string? ReportingUrl { get; set; }

    /// <summary>
    /// Use Xss Protection Policy Header.
    /// </summary>
    [Required]
    public virtual XXssProtectionPolicyBlockMode XssProtectionPolicyHeader { get; set; } = XXssProtectionPolicyBlockMode.FilterDisabled;
}