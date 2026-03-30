using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetExternalLoginsRequest : GetExternalLoginsRequest<Guid>;

/// <summary>
/// Request to get external login providers for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[GetAction(ActionRoutes.IDENTITY_EXTERNAL_LOGINS)]
public class GetExternalLoginsRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;
}