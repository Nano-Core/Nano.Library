using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Base request for adding an external login.
/// </summary>
public abstract class BaseAddExternalLoginRequest(string providerName) : BaseRequest
{
    /// <summary>
    /// The provider name of the the external provider.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual string ProviderName { get; } = providerName ?? throw new ArgumentNullException(nameof(providerName));
}

/// <summary>
/// Base request for adding an external login.
/// </summary>
/// <typeparam name="TFlow">Type of the external login flow.</typeparam>
/// <typeparam name="TIdentity">Type of identity of the user.</typeparam>
[PostAction(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD)]
public abstract class BaseAddExternalLoginRequest<TFlow, TIdentity>(string providerName) : BaseAddExternalLoginRequest(providerName)
    where TFlow : BaseAuthFlow
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// The add external login.
    /// </summary>
    [Required]
    [Body]
    public virtual required AddExternalLogin<TFlow> AddExternalLogin { get; set; }
}