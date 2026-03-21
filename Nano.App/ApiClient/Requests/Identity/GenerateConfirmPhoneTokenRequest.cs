using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmPhoneTokenRequest : GenerateConfirmPhoneTokenRequest<Guid>;

/// <summary>
/// Request to generate a confirm phone token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_PHONE_CONFIRM_TOKEN)]
public class GenerateConfirmPhoneTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user id.
    /// </summary>
    [Required]
    [Route]
    public virtual TIdentity Id { get; set; } = default!;
}