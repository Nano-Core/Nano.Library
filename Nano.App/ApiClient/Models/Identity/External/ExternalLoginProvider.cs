using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity.External;

/// <summary>
/// External Login Provider.
/// </summary>
public class ExternalLoginProvider
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Display Name.
    /// </summary>
    [Required]
    public virtual string DisplayName { get; set; }
}