using System;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveClaimRequest : RemoveClaimRequest<Guid>;

/// <inheritdoc />
public class RemoveClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Assign Claim.
    /// </summary>
    public virtual RemoveRole<TIdentity> RemoveClaim { get; set; } = new();

    /// <inheritdoc />
    public RemoveClaimRequest()
    {
        this.Action = "claim/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveClaim;
    }
}