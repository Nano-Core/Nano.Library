using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserClaimRequest : RemoveUserClaimRequest<Guid>;

/// <inheritdoc />
public class RemoveUserClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Remove Claim.
    /// </summary>
    public virtual RemoveUserClaim<TIdentity> RemoveUserClaim { get; set; } = new();

    /// <inheritdoc />
    public RemoveUserClaimRequest()
    {
        this.Action = "claims/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveUserClaim;
    }
}