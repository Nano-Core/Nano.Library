using System;
using System.Linq.Expressions;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Base Criteria (abstract).
    /// </summary>
    public abstract class BaseCriteria : ICriteria
    {
        /// <inheritdoc />
        public abstract Expression<Func<TEntity, bool>> GetExpression<TEntity>()
            where TEntity : IEntity;
    }
}