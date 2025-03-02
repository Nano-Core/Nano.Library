using System;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveExternalLoginRequest : RemoveExternalLoginRequest<Guid>;

/// <inheritdoc />
public class RemoveExternalLoginRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; }

    /// <inheritdoc />
    public RemoveExternalLoginRequest()
    {
        this.Action = "external-logins/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}