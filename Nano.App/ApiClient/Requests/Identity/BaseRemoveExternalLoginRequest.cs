using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a base request to remove an external login from a user.
/// </summary>
public abstract class BaseRemoveExternalLoginRequest(string providerName) : BaseRequest
{
    /// <summary>
    /// The provider name of the the external provider.
    /// </summary>
    [Required]
    [Route(Order = 1)]
    public virtual string ProviderName { get; } = providerName ?? throw new ArgumentNullException(nameof(providerName));
}

/// <summary>
/// Represents a request to remove an external login from a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE)]
public class BaseRemoveExternalLoginRequest<TIdentity>(string providerName) : BaseRemoveExternalLoginRequest(providerName)
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}