using System;
using DynamicExpression;
using DynamicExpression.Interfaces;

namespace Nano.Models.Criterias
{
    /// <inheritdoc />
    public class DefaultQueryCriteria : IQueryCriteria
    {
        /// <summary>
        /// Is Active (read-only).
        /// Default: true.
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
        public virtual CriteriaExpression GetExpression<TEntity>() 
            where TEntity : class
        {
            var expression = new CriteriaExpression();

            expression.Equal("IsActive", this.IsActive);

            if (this.BeforeAt.HasValue)
                expression.LessThanOrEqual("CreatedAt", this.BeforeAt);

            if (this.AfterAt.HasValue)
                expression.GreaterThanOrEqual("CreatedAt", this.AfterAt);

            return expression;
        }
    }
}