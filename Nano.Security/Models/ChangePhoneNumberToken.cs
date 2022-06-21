using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Change Phone Number Token
/// </summary>
public class ChangePhoneNumberToken
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// Phone Number.
    /// </summary>
    [Required]
    public virtual string PhoneNumber { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [Required]
    public virtual string NewPhoneNumber { get; set; }
}