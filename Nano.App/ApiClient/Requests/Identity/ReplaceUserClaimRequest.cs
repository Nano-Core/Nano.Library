using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceUserClaimRequest : ReplaceUserClaimRequest<Guid>;

/// <summary>
/// Represents a request to replace a user's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class ReplaceUserClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user claim replacement information.
    /// </summary>
    public virtual ReplaceUserClaim<TIdentity> ReplaceUserClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ReplaceUserClaimRequest{TIdentity}"/> with action set.
    /// </summary>
    public ReplaceUserClaimRequest()
    {
        this.Action = "claims/replace";
    }

    /// <summary>
    /// Gets the body of the request containing the claim replacement data.
    /// </summary>
    public override object GetBody()
    {
        return this.ReplaceUserClaim;
    }
}