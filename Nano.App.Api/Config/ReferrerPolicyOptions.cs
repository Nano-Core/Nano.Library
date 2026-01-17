using System.ComponentModel.DataAnnotations;
using Nano.App.Api.Config.Enums;

namespace Nano.App.Api.Config;

/// <summary>
/// 
/// </summary>
public class ReferrerPolicyOptions
{
    /// <summary>
    /// Use Referrer Policy Header.
    /// </summary>
    [Required]
    public virtual ReferrerPolicy ReferrerPolicyHeader { get; set; } = ReferrerPolicy.Disabled;
}