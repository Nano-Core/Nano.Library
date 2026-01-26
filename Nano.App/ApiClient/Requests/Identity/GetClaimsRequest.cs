using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetClaimsRequest : GetClaimsRequest<Guid>;

/// <summary>
/// Request to retrieve claims for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GetClaimsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetClaimsRequest{TIdentity}"/>.
    /// Sets the action to "claims".
    /// </summary>
    public GetClaimsRequest()
    {
        this.Action = "claims";
    }
}