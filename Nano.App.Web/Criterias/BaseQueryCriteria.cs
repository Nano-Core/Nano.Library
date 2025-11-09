using System.Collections.Generic;
using DynamicExpression;
using DynamicExpression.Interfaces;

namespace Nano.Models.Criterias;

/// <inheritdoc />
public abstract class BaseQueryCriteria : IQueryCriteria
{
    /// <inheritdoc />
    public virtual IList<CriteriaExpression> GetExpressions()
    {
        return new List<CriteriaExpression>();
    }
}