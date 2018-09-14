using DynamicExpression;
using DynamicExpression.Interfaces;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public abstract class BaseQueryCriteria : IQueryCriteria
    {
        /// <inheritdoc />
        public virtual CriteriaExpression GetExpression()
        {
            var expression = new CriteriaExpression();

            return expression;
        }
    }
}