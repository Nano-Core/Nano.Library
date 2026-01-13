using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Provider.
/// </summary>
public class ExternalLoginProvider
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Display Name.
    /// </summary>
    public virtual string? DisplayName { get; set; }
}