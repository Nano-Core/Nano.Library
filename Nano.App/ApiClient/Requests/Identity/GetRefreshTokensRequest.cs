using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetRefreshTokensRequest : GetRefreshTokensRequest<Guid>;

/// <summary>
/// Represents a request to retrieve refresh tokens for a specific user.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_REFRESH_TOKENS)]
public class GetRefreshTokensRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The user identifier.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;
}