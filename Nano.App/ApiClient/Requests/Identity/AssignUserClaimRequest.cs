using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignUserClaimRequest : AssignUserClaimRequest<Guid>;

/// <summary>
/// Request to assign a claim to a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class AssignUserClaimRequest<TIdentity> : BaseRequestPost
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user claim assignment information.
    /// </summary>
    public virtual AssignUserClaim<TIdentity> AssignUserClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AssignUserClaimRequest{TIdentity}"/>.
    /// Sets the action to "claims/assign".
    /// </summary>
    public AssignUserClaimRequest()
    {
        this.Action = "claims/assign";
    }

    /// <summary>
    /// Gets the request body containing the claim assignment information.
    /// </summary>
    public override object GetBody()
    {
        return this.AssignUserClaim;
    }
}