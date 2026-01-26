using Nano.Data.Abstractions.Identity.Models;
using System;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AssignOrReplaceUserClaimRequest : AssignOrReplaceUserClaimRequest<Guid>;

/// <summary>
/// Request to assign or replace a claim for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class AssignOrReplaceUserClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The claim assignment or replacement information.
    /// </summary>
    public virtual AssignOrReplaceUserClaim<TIdentity> AssignOrReplaceUserClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AssignOrReplaceUserClaimRequest{TIdentity}"/>.
    /// Sets the action to "claims/assign-or-replace".
    /// </summary>
    public AssignOrReplaceUserClaimRequest()
    {
        this.Action = "claims/assign-or-replace";
    }

    /// <summary>
    /// Gets the request body containing the claim assignment or replacement information.
    /// </summary>
    public override object GetBody()
    {
        return this.AssignOrReplaceUserClaim;
    }
}