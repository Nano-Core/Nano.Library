using Nano.Data.Abstractions.Identity.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceUserClaimRequest : ReplaceUserClaimRequest<Guid>;

/// <summary>
/// Represents a request to replace a user's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[PutAction(ActionRoutes.IDENTITY_USER_CLAIMS_REPLACE)]
public class ReplaceUserClaimRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user claim replacement information.
    /// </summary>
    [Required]
    [Body]
    public virtual ReplaceClaim ReplaceClaim { get; set; } = new();
}