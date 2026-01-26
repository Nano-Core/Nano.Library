using System;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class ReplaceRoleClaimRequest : ReplaceRoleClaimRequest<Guid>;

/// <summary>
/// Represents a request to replace a role's claim.
/// </summary>
/// <typeparam name="TIdentity">The type of the role identifier.</typeparam>
public class ReplaceRoleClaimRequest<TIdentity> : BaseRequestPut
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Contains the role claim replacement information.
    /// </summary>
    public virtual ReplaceRoleClaim<TIdentity> ReplaceRoleClaim { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ReplaceRoleClaimRequest{TIdentity}"/> with action set.
    /// </summary>
    public ReplaceRoleClaimRequest()
    {
        this.Action = "roles/claims/replace";
    }

    /// <summary>
    /// Gets the body of the request containing the role claim replacement data.
    /// </summary>
    public override object GetBody()
    {
        return this.ReplaceRoleClaim;
    }
}