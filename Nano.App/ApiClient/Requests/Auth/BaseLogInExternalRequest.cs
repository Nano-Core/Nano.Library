using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Base class for external login requests.
/// </summary>
public abstract class BaseLogInExternalRequest(string providerName) : BaseAuthRequest
{
    /// <summary>
    /// The provider name of the the external login provider.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual string ProviderName { get; } = providerName ?? throw new ArgumentNullException(nameof(providerName));
}

/// <summary>
/// Base class for external login requests.
/// </summary>
[PostAction(ActionRoutes.AUTH_LOGIN_EXTERNAL)]
public abstract class BaseLogInExternalRequest<TFlow>(string providerName) : BaseLogInExternalRequest(providerName)
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// Contains the external login data for the request.
    /// </summary>
    [Required]
    [Body]
    public virtual required LogInExternal<TFlow> LoginExternal { get; set; }
}