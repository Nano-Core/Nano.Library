using Nano.Models.Criterias.Interfaces;
using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests.Spatial;

/// <summary>
/// Within Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
public class WithinRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteriaSpatial, new()
{
    /// <summary>
    /// Distance.
    /// </summary>
    [Query]
    public virtual int Distance { get; set; }

    /// <inheritdoc />
    public WithinRequest()
    {
        this.Action = "within";
    }
}