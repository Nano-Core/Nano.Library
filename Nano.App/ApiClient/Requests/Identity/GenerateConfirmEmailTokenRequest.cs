using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateConfirmEmailTokenRequest : GenerateConfirmEmailTokenRequest<Guid>;

/// <summary>
/// Request to generate a confirm email token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_EMAIL_CONFIRM_TOKEN)]
public class GenerateConfirmEmailTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The confirm email token information.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;
}