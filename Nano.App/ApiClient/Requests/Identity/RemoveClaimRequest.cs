using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveClaimRequest : RemoveClaimRequest<Guid>;

/// <inheritdoc />
public class RemoveClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Remove Claim.
    /// </summary>
    public virtual RemoveClaim<TIdentity> RemoveClaim { get; set; } = new();

    /// <inheritdoc />
    public RemoveClaimRequest()
    {
        this.Action = "claims/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.RemoveClaim;
    }
}