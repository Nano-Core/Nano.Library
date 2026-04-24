using DynamicExpression;
using DynamicExpression.Interfaces;
using Nano.Data.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace Nano.App.Api.Controllers.Criteria;

/// <summary>
/// Base implementation of <see cref="IQueryCriteria"/> with time-based filtering options.
/// </summary>
public abstract class BaseQueryCriteria : IQueryCriteria
{
    /// <summary>
    /// Only include records that is created after this date and time (inclusive).
    /// </summary>
    public virtual DateTimeOffset? CreatedAfter { get; set; }

    /// <summary>
    /// Only include records that is created before this date and time (inclusive).
    /// </summary>
    public virtual DateTimeOffset? CreatedBefore { get; set; }

    /// <summary>
    /// Builds the list of <see cref="CriteriaExpression"/> instances based on the properties of this query.
    /// </summary>
    /// <returns>A list of <see cref="CriteriaExpression"/> representing the query conditions.</returns>
    public virtual IList<CriteriaExpression> GetExpressions()
    {
        var expressions = new List<CriteriaExpression>();

        var expression = new CriteriaExpression();

        if (this.CreatedBefore.HasValue)
        {
            expression
                .LessThanOrEqual(nameof(BaseEntity.CreatedAt), this.CreatedBefore);
        }

        if (this.CreatedAfter.HasValue)
        {
            expression
                .GreaterThanOrEqual(nameof(BaseEntity.CreatedAt), this.CreatedAfter);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}