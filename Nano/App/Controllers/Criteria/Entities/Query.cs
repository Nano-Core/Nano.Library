using System;
using System.Linq.Expressions;
using Nano.App.Controllers.Criteria.Interfaces;
using Nano.App.Models.Interfaces;

namespace Nano.App.Controllers.Criteria.Entities
{
    /// <summary>
    /// Default Query.
    /// </summary>
    public class Criteria : ICriteria
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
        public virtual Expression<Func<TEntity, bool>> GetExpression<TEntity>() 
            where TEntity : IEntity
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var isActive = Expression.Equal(Expression.Property(parameter, "IsActive"), Expression.Constant(this.IsActive));
            var expression = Expression.Lambda<Func<TEntity, bool>>(isActive, parameter);

            if (this.AfterAt.HasValue)
            {
                var property = Expression.Property(parameter, "CreatedAt");
                var afterAt = Expression.GreaterThanOrEqual(property, Expression.Constant(this.AfterAt.Value));
                expression =  Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression.Body, afterAt), parameter);
            }

            if (this.BeforeAt.HasValue)
            {
                var property = Expression.Property(parameter, "CreatedAt");
                var beforeAt = Expression.LessThanOrEqual(property, Expression.Constant(this.BeforeAt.Value));
                expression = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(expression.Body, beforeAt), parameter);
            }

            return expression;
        }
    }
}