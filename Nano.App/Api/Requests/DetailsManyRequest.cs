using System;
using System.Collections.Generic;

namespace Nano.App.Api.Requests;

/// <inheritdoc />
public class DetailsManyRequest : DetailsManyRequest<Guid>;

/// <summary>
/// Details Many Request.
/// </summary>
public class DetailsManyRequest<TIdentity> : BaseRequestPost
{
    /// <summary>
    /// Ids.
    /// </summary>
    public virtual ICollection<TIdentity> Ids { get; set; }

    /// <summary>
    /// Constructor.
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