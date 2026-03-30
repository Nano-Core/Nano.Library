using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmPhoneRequest : ConfirmPhoneRequest<Guid>;

/// <summary>
/// Request to confirm a user's phone number.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PHONE_CONFIRM)]
public class ConfirmPhoneRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm phone token information.
    /// </summary>
    [Required]
    [Body]
    public virtual ConfirmPhoneNumber ConfirmPhone { get; set; } = new();
}