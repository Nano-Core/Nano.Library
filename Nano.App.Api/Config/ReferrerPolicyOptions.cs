using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring Referrer-Policy headers.
/// </summary>
public class ReferrerPolicyOptions
{
    /// <summary>
    /// Specifies the Referrer-Policy header value.
    /// </summary>
    [Required]
    public virtual ReferrerPolicy ReferrerPolicyHeader { get; set; } = ReferrerPolicy.Disabled;
}