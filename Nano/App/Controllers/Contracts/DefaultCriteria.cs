using System;
using System.Linq.Expressions;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Default Criteria.
    /// </summary>
    public class DefaultCriteria : BaseCriteria
    {
        /// <summary>
        /// Is Active.
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
        public override Expression<Func<TEntity, bool>> GetExpression<TEntity>()
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var property = Expression.Property(parameter, "CreatedAt");
            var value = Expression.Equal(Expression.Property(parameter, "IsActive"), Expression.Constant(this.IsActive));
            var expression = Expression.Lambda<Func<TEntity, bool>>(value, parameter);

            if (this.AfterAt.HasValue)
            {
                var afterAt = Expression.GreaterThanOrEqual(property, Expression.Constant(this.AfterAt.Value));
                expression =  Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression.Body, afterAt), parameter);
            }

            if (this.BeforeAt.HasValue)
            {
                var beforeAt = Expression.LessThanOrEqual(property, Expression.Constant(this.BeforeAt.Value));
                expression = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression.Body, beforeAt), parameter);
            }

            return expression;
        }
    }
}