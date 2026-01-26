using System;
using System.Collections.Generic;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DetailsManyRequest : DetailsManyRequest<Guid>;

/// <summary>
/// Represents a request to get details of multiple entities.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
public class DetailsManyRequest<TIdentity> : BaseRequestPost
{
    /// <summary>
    /// The IDs of the entities.
    /// </summary>
    public virtual ICollection<TIdentity> Ids { get; set; } = [];

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="DetailsManyRequest{TIdentity}"/>.
    /// </summary>
    public DetailsManyRequest()
    {
        this.Action = "details/many";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Ids;
    }
}