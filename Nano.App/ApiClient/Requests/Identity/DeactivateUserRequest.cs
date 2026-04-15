using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using System;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeactivateUserRequest : DeactivateUserRequest<Guid>;

/// <summary>
/// Request to deactivate a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[DeleteAction(ActionRoutes.IDENTITY_DEACTIVATE)]
public class DeactivateUserRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user to deactivate.
    /// </summary>
    [Route(Order = 0)]
    public virtual required TIdentity Id { get; set; }
}