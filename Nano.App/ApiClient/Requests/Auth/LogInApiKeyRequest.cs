using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an api-key.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_API_KEY)]
public class LogInApiKeyRequest : BaseAuthRequest
{
    /// <summary>
    /// Contains the login details for apikey.
    /// </summary>
    [Body]
    [Required]
    public virtual required LogInApiKey LogInApiKey { get; set; }
}