using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;

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
    /// Contains the custom purpose token to verify.
    /// </summary>
    [Required]
    [Body]
    public virtual ConfirmCustomPurpose<TIdentity> ConfirmCustomPurpose { get; set; } = new();
}