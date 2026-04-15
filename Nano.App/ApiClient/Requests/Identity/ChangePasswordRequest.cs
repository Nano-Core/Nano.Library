using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangePasswordRequest : ChangePasswordRequest<Guid>;

/// <summary>
/// Request to change a user's password.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PASSWORD_CHANGE)]
public class ChangePasswordRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change password token information.
    /// </summary>
    [Required]
    [Body]
    public virtual required ChangePassword ChangePassword { get; set; }
}