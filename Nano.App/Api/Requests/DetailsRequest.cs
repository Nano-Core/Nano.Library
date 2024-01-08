using System;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests;

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
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public DetailsRequest()
    {
        this.Action = "details";
    }
}