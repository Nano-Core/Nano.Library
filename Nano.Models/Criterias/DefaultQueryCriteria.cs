using System;
using DynamicExpression;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public class DefaultQueryCriteria : BaseQueryCriteria
    {
        /// <summary>
        /// Is Active.
        /// Default: true. 
        /// WHen soft-deleted it's set to false instead of deleting data.
        /// </summary>
        public virtual bool IsActive { get; } = true;

        /// <summary>
        /// After At.
        /// </summary>
        public virtual DateTimeOffset? AfterAt { get; set; }

        /// <summary>
        /// Before At.
        /// </summary>
        public virtual DateTimeOffset? BeforeAt { get; set; }

        /// <inheritdoc />
        public override CriteriaExpression GetExpression<TEntity>() 
        {
            var expression = base.GetExpression<TEntity>();

            expression.Equal("IsActive", this.IsActive);

            if (this.BeforeAt.HasValue)
                expression.LessThanOrEqual("CreatedAt", this.BeforeAt);

            if (this.AfterAt.HasValue)
                expression.GreaterThanOrEqual("CreatedAt", this.AfterAt);

            return expression;
        }
    }
}