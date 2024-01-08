using Nano.App.Api.Requests.Attributes;
using Nano.Models.Criterias.Interfaces;

namespace Nano.App.Api.Requests.Spatial;

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