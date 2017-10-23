using System;
using System.Linq.Expressions;
using Nano.App.Models.Interfaces;

namespace Nano.App.Controllers.Criteria.Interfaces
{
    /// <summary>
    /// Query interface.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Gets the <see cref="Expression{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
        /// <returns>The <see cref="Expression{TDelegate}"/></returns>
        Expression<Func<TEntity, bool>> GetExpression<TEntity>()
            where TEntity : IEntity;
    }
}