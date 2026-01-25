using Nano.App.Api.Config.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring X-XSS-Protection headers.
/// </summary>
public class XXssProtectionOptions
{
    /// <summary>
    /// URL to report XSS attempts.
    /// </summary>
    public virtual string? ReportingUrl { get; set; }

    /// <summary>
    /// Specifies the X-XSS-Protection policy header value.
    /// </summary>
    [Required]
    public virtual XXssProtectionPolicyBlockMode XssProtectionPolicyHeader { get; set; } = XXssProtectionPolicyBlockMode.FilterDisabled;
}