using System;
using System.Collections.Generic;
using DynamicExpression;
using DynamicExpression.Interfaces;
using Nano.Data.Abstractions.Models;

namespace Nano.App.Api.Controllers.Criteria;

/// <summary>
/// Base implementation of <see cref="IQueryCriteria"/> with time-based filtering options.
/// </summary>
public abstract class BaseQueryCriteria : IQueryCriteria
{
    /// <summary>
    /// Filter for records created after this date and time (inclusive).
    /// </summary>
    public virtual DateTimeOffset? AfterAt { get; set; }

    /// <summary>
    /// Filter for records created before this date and time (inclusive).
    /// </summary>
    public virtual DateTimeOffset? BeforeAt { get; set; }

    /// <summary>
    /// Builds the list of <see cref="CriteriaExpression"/> instances based on the properties of this query.
    /// </summary>
    /// <returns>A list of <see cref="CriteriaExpression"/> representing the query conditions.</returns>
    public virtual IList<CriteriaExpression> GetExpressions()
    {
        var expressions = new List<CriteriaExpression>();

        var expression = new CriteriaExpression();

        if (this.BeforeAt.HasValue)
        {
            expression
                .LessThanOrEqual(nameof(BaseEntity.CreatedAt), this.BeforeAt);
        }

        if (this.AfterAt.HasValue)
        {
            expression
                .GreaterThanOrEqual(nameof(BaseEntity.CreatedAt), this.AfterAt);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}