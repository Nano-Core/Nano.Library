using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetExternalLoginsRequest : GetExternalLoginsRequest<Guid>;

/// <inheritdoc />
public class GetExternalLoginsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <inheritdoc />
    public GetExternalLoginsRequest()
    {
        this.Action = "external-logins";
    }
}