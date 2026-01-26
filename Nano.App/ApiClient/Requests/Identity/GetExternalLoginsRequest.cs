using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetExternalLoginsRequest : GetExternalLoginsRequest<Guid>;

/// <summary>
/// Request to get external login providers for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GetExternalLoginsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetExternalLoginsRequest"/>.
    /// Sets the action to "external-logins".
    /// </summary>
    public GetExternalLoginsRequest()
    {
        this.Action = "external-logins";
    }
}