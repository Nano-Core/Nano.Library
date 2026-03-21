using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GenerateCustomPurposeTokenRequest : GenerateCustomPurposeTokenRequest<Guid>;

/// <summary>
/// Request to generate a custom purpose token for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_CUSTOM_PURPOSE_CONFIRM_TOKEN)]
public class GenerateCustomPurposeTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The custom purpose token information.
    /// </summary>
    [Required]
    [Body]
    public virtual GenerateCustomPurposeToken CustomPurposeToken { get; set; } = new();
}