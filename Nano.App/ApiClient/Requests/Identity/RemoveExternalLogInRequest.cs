using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveExternalLoginRequest : RemoveExternalLoginRequest<Guid>;

/// <summary>
/// Represents a request to remove an external login from a user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
//[DeleteAction(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE_DIRECT)]
public class RemoveExternalLoginRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the external login removal information.
    /// </summary>
    [Required]
    [Body]
    public virtual RemoveExternalLogin RemoveExternalLogin { get; set; } = new();
}