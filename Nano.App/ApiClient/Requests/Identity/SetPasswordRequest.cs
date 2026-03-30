using Nano.Data.Abstractions.Identity.Models;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to set a user's password.
/// </summary>
[PostAction(ActionRoutes.IDENTITY_PASSWORD_SET)]
public class SetPasswordRequest : BaseRequest
{
    /// <summary>
    /// Contains the password information to set.
    /// </summary>
    [Required]
    [Body]
    public virtual SetPassword SetPassword { get; set; } = new();
}