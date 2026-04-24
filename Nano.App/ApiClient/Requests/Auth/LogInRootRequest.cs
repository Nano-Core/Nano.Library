using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in as a root user.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_ROOT)]
public class LogInRootRequest : BaseAuthRequest
{
    /// <summary>
    /// Contains the login details for root login.
    /// </summary>
    [Body]
    [Required]
    public virtual required LogInRoot LogInRoot { get; set; }
}