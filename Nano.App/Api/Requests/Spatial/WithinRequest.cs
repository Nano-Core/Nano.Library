using DynamicExpression.Interfaces;
using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Spatial;

/// <summary>
/// Within Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class WithinRequest<TCriteria> : BaseSpatialRequest<TCriteria>
    where TCriteria : IQueryCriteria, new()
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