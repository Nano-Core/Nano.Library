using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangeEmailRequest : ChangeEmailRequest<Guid>;

/// <summary>
/// Request to change a user's email address.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_EMAIL_CHANGE)]
public class ChangeEmailRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change email token information.
    /// </summary>
    [Required]
    [Body]
    public virtual required ChangeEmail ChangeEmail { get; set; }

    /// <summary>
    /// Indicates whether to also set the username when changing email.
    /// </summary>
    [Required]
    [Query]
    public virtual bool SetUsername { get; set; } = false;
}