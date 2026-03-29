using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ResetPasswordRequest : ResetPasswordRequest<Guid>;

/// <summary>
/// Represents a request to reset a user's password.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PASSWORD_RESET)]
public class ResetPasswordRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the reset password information.
    /// </summary>
    [Required]
    [Body]
    public virtual ResetPassword ResetPassword { get; set; } = new();
}