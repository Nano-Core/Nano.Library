using Nano.App.Api.Requests.Attributes;
using System;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetExternalLoginsRequest : GetExternalLoginsRequest<Guid>;

/// <inheritdoc />
public class GetExternalLoginsRequest<TIdentity> : BaseRequestGet
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; }

    /// <inheritdoc />
    public GetExternalLoginsRequest()
    {
        this.Action = "external-logins";
    }
}