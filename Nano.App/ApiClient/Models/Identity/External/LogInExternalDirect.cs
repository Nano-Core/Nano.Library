using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Models.Identity.External;

/// <summary>
/// Log In External Direct.
/// </summary>
public class LogInExternalDirect : LogInExternal
{
    /// <summary>
    /// External LogIn Data.
    /// </summary>
    public virtual ExternalLogInData ExternalLogInData { get; set; }
}