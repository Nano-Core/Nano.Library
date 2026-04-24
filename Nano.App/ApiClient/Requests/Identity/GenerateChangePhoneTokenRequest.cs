using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangePhoneTokenRequest : GenerateChangePhoneTokenRequest<Guid>;

/// <summary>
/// Request to generate a change phone token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PHONE_CHANGE_TOKEN)]
public class GenerateChangePhoneTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// The change phone token information.
    /// </summary>
    [Required]
    [Body]
    public virtual required GenerateChangePhoneToken ChangePhoneToken { get; set; }
}