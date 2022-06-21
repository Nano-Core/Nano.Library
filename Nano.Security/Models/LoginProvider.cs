using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// LogIn Provider.
/// </summary>
public class LogInProvider
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