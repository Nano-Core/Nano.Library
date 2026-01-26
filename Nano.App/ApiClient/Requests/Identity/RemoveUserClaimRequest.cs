using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class RemoveUserClaimRequest : RemoveUserClaimRequest<Guid>;

/// <summary>
/// Represents a request to remove a user's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the user identifier.</typeparam>
public class RemoveUserClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the user claim removal information.
    /// </summary>
    public virtual RemoveUserClaim<TIdentity> RemoveUserClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="RemoveUserClaimRequest{TIdentity}"/> with action set.
    /// </summary>
    public RemoveUserClaimRequest()
    {
        this.Action = "claims/remove";
    }

    /// <summary>
    /// Gets the body of the request containing the user claim removal data.
    /// </summary>
    public override object GetBody()
    {
        return this.RemoveUserClaim;
    }
}