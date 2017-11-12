using Nano.Controllers.Criterias;
using Nano.Controllers.Criterias.Entities;

namespace Nano.Example.Controllers.Criterias
{
    /// <inheritdoc />
    public class ExampleCriteria : DefaultCriteria
    {
        /// <summary>
        /// Required.
        /// Property One.
        /// </summary>
        public virtual string PropertyOne { get; set; }

        /// <inheritdoc />
        public override Filter GetExpression<TEntity>()
        {
            var filter = base.GetExpression<TEntity>();

            if (this.PropertyOne != null)
                filter.StartsWith("PropertyOne", this.PropertyOne);

            return filter;
        }
    }
}