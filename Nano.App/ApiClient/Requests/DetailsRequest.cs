using System;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DetailsRequest : DetailsRequest<Guid>;

/// <summary>
/// Represents a request to get details of a single entity by ID.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifier.</typeparam>
public class DetailsRequest<TIdentity> : BaseRequestGet
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The ID of the entity.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="DetailsRequest{TIdentity}"/>.
    /// </summary>
    public DetailsRequest()
    {
        this.Action = "details";
    }
}