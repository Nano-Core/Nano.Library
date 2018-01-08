using DynamicExpression;
using DynamicExpression.Interfaces;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public abstract class BaseQueryCriteria : IQueryCriteria
    {
        /// <inheritdoc />
        public virtual CriteriaExpression GetExpression<TEntity>()
            where TEntity : class
        {
            var expression = new CriteriaExpression();

            return expression;
        }
    }
}