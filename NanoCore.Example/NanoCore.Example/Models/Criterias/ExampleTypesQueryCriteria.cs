using DynamicExpression;
using Nano.Models.Criterias;

namespace NanoCore.Example.Models.Criterias
{
    /// <inheritdoc />
    public class ExampleTypesQueryCriteria : DefaultQueryCriteria
    {
        /// <inheritdoc />
        public override CriteriaExpression GetExpression<TEntity>()
        {
            var filter = base.GetExpression<TEntity>();

            return filter;
        }
    }
}