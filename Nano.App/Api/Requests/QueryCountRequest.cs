﻿using DynamicExpression.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Query Count Request.
/// </summary>
/// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
public class QueryCountRequest<TCriteria> : BaseRequestPost
    where TCriteria : IQueryCriteria, new()
{
    /// <summary>
    /// Criteria.
    /// </summary>
    public virtual TCriteria Criteria { get; set; } = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    public QueryCountRequest()
    {
        this.Action = "query/count";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Criteria;
    }
}