using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeleteUserRefreshTokenRequest : DeleteUserRefreshTokenRequest<Guid>;

/// <inheritdoc />
public class DeleteUserRefreshTokenRequest<TIdentity> : BaseRequestDelete
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity RefreshTokenId { get; set; } = default!;

    /// <inheritdoc />
    public DeleteUserRefreshTokenRequest()
    {
        this.Action = "refresh-tokens/delete";
    }

    /// <inheritdoc />
    public override object? GetBody()
    {
        return null;
    }
}