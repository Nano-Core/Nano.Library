using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetClaimsRequest : GetClaimsRequest<Guid>;

/// <summary>
/// Request to retrieve claims for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_USER_CLAIMS)]
public class GetClaimsRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual required TIdentity UserId { get; set; }
}