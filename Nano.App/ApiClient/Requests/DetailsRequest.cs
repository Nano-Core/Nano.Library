using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DetailsRequest : DetailsRequest<Guid>;

/// <summary>
/// Details Request.
/// </summary>
public class DetailsRequest<TIdentity> : BaseRequestGet
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Include Depth.
    /// </summary>
    public virtual int? IncludeDepth { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public DetailsRequest()
    {
        this.Action = "details";
    }
}