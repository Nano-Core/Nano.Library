using System;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeleteUserRefreshTokenRequest : DeleteUserRefreshTokenRequest<Guid>;

/// <summary>
/// Request to delete a user's refresh token.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_REFRESH_TOKENS_DELETE)]
public class DeleteUserRefreshTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the refresh token to delete.
    /// </summary>
    [Route(Order = 0)]
    public virtual required TIdentity RefreshTokenId { get; set; }
}