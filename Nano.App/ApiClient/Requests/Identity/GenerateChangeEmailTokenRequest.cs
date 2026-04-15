using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateChangeEmailTokenRequest : GenerateChangeEmailTokenRequest<Guid>;

/// <summary>
/// Request to generate a change email token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_EMAIL_CHANGE_TOKEN)]
public class GenerateChangeEmailTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The change email token information.
    /// </summary>
    [Required]
    [Body]
    public virtual required GenerateChangeEmailToken ChangeEmailToken { get; set; }
}