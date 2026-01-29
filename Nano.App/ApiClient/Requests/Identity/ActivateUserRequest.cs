using System;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ActivateUserRequest : ActivateUserRequest<Guid>;

/// <summary>
/// Request to activate a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
[PostAction(ActionRoutes.IDENTITY_ACTIVATE)]
public class ActivateUserRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The identifier of the user to activate.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;
}