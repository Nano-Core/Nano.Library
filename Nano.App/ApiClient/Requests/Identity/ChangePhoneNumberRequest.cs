using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ChangePhoneRequest : ChangePhoneRequest<Guid>;

/// <summary>
/// Request to change a user's phone number.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PHONE_CHANGE)]
public class ChangePhoneRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change phone token information.
    /// </summary>
    [Required]
    [Body]
    public virtual ChangePhoneNumber ChangePhone { get; set; } = new();
}