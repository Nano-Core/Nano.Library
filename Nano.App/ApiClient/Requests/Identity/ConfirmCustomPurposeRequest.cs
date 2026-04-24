using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ConfirmCustomPurposeRequest : ConfirmCustomPurposeRequest<Guid>;

/// <summary>
/// Represents a request to verify a custom-purpose token.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_CUSTOM_PURPOSE_CONFIRM)]
public class ConfirmCustomPurposeRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }

    /// <summary>
    /// Contains the custom purpose token to verify.
    /// </summary>
    [Required]
    [Body]
    public virtual required ConfirmCustomPurpose ConfirmCustomPurpose { get; set; }
}