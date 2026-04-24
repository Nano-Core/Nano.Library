using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Base request for signing up a user via an external provider.
/// </summary>
public abstract class BaseSignUpExternalRequest(string providerName) : BaseRequest
{
    /// <summary>
    /// The provider name of the the external provider.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual string ProviderName { get; } = providerName ?? throw new ArgumentNullException(nameof(providerName));
}

/// <summary>
/// Base request for signing up a user via an external provider.
/// </summary>
/// <typeparam name="TFlow">Type of the sign-up flow.</typeparam>
/// <typeparam name="TUser">Type of user.</typeparam>
/// <typeparam name="TIdentity">Type of identity of the user.</typeparam>
[PostAction(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL)]
public abstract class BaseSignUpExternalRequest<TFlow, TUser, TIdentity>(string providerName) : BaseSignUpExternalRequest(providerName)
    where TFlow : BaseAuthFlow
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The external sign-up information.
    /// </summary>
    [Required]
    [Body]
    public virtual required SignUpExternal<TFlow, TUser, TIdentity> SignUpExternal { get; set; }
}