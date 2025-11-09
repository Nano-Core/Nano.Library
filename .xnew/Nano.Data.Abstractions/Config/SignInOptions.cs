namespace Nano.Data;

/// <summary>
/// Sign-In Options (nested class).
/// </summary>
public class SignInOptions
{
    /// <summary>
    /// Require Confirmed Email-
    /// </summary>
    public virtual bool RequireConfirmedEmail { get; set; } = false;

    /// <summary>
    /// Require Confirmed PhoneNumber.
    /// </summary>
    public virtual bool RequireConfirmedPhoneNumber { get; set; } = false;
}