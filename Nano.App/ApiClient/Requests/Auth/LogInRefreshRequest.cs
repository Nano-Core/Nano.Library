using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to refresh an existing login session.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_REFRESH)]
public class LogInRefreshRequest : BaseAuthRequest
{
    /// <summary>
    /// Contains the refresh login details.
    /// </summary>
    [Required]
    [Body]
    public virtual LogInRefresh LogInRefresh { get; set; } = new();
}