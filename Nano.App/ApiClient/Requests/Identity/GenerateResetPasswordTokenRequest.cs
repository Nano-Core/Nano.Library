using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to generate a reset password token.
/// </summary>
[PostAction(ActionRoutes.IDENTITY_PASSWORD_RESET_TOKEN)]
public class GenerateResetPasswordTokenRequest : BaseRequest
{
    /// <summary>
    /// The reset password token information.
    /// </summary>
    [Required]
    [Body]
    public virtual GenerateResetPasswordToken ResetPasswordToken { get; set; } = new();
}