using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class SetPasswordRequest : SetPasswordRequest<Guid>;

/// <summary>
/// Represents a request to set a user's password.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PASSWORD_SET)]
public class SetPasswordRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// Contains the password information to set.
    /// </summary>
    [Required]
    [Body]
    public virtual required SetPassword SetPassword { get; set; }
}