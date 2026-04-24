using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeleteRefreshTokenRequest : DeleteRefreshTokenRequest<Guid>;

/// <summary>
/// Request to delete a user's refresh token.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_REFRESH_TOKENS_DELETE)]
public class DeleteRefreshTokenRequest<TIdentity> : BaseRequest
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}