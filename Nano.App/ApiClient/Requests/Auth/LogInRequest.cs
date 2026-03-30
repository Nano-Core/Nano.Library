using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in a standard user.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN)]
public class LogInRequest : BaseAuthRequest
{
    /// <summary>
    /// Contains the login details for standard login.
    /// </summary>
    [Body]
    [Required]
    public virtual LogIn LogIn { get; set; } = new();
}