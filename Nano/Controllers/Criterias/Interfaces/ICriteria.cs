using System.Linq.Expressions;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Criterias.Interfaces
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
        Filter GetExpression<TEntity>()
            where TEntity : class, IEntity;
    }
}