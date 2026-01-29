using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmEmailRequest : ConfirmEmailRequest<Guid>;

/// <summary>
/// Request to confirm a user's email address.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_EMAIL_CONFIRM)]
public class ConfirmEmailRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm email token information.
    /// </summary>
    [Required]
    [Body]
    public virtual ConfirmEmail<TIdentity> ConfirmEmail { get; set; } = new();
}