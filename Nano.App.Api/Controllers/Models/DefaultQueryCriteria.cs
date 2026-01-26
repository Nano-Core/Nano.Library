using System;
using System.Collections.Generic;
using DynamicExpression;
using Nano.Data.Abstractions.Models;

namespace Nano.App.Api.Controllers.Models;

/// <summary>
/// Default implementation of <see cref="BaseQueryCriteria"/> with time-based filtering options.
/// </summary>
public class DefaultQueryCriteria : BaseQueryCriteria
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
    public override IList<CriteriaExpression> GetExpressions()
    {
        var expressions = base.GetExpressions();

        var expression = new CriteriaExpression();

        if (this.BeforeAt.HasValue)
        {
            expression
                .LessThanOrEqual(nameof(DefaultEntity.CreatedAt), this.BeforeAt);
        }

        if (this.AfterAt.HasValue)
        {
            expression
                .GreaterThanOrEqual(nameof(DefaultEntity.CreatedAt), this.AfterAt);
        }

        expressions
            .Add(expression);

        return expressions;
    }
}