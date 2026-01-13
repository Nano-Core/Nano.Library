using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Log In External Direct.
/// </summary>
public class LogInExternalDirect : LogInExternal
{
    /// <summary>
    /// External LogIn Data.
    /// </summary>
    [Required]
    public virtual ExternalLogInData ExternalLogInData { get; set; } = new();
}