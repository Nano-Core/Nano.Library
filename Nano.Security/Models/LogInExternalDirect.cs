namespace Nano.Security.Models;

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