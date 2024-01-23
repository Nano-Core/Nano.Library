using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Generate Reset Password Token.
/// </summary>
public class GenerateResetPasswordToken
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string EmailAddress { get; set; }
}