using System;
using System.Linq;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Contracts
{
    /// <inheritdoc />
    public class Criteria : ICriteria
    {
        /// <inheritdoc />
        public virtual bool IsActive { get; } = true;

        /// <inheritdoc />
        public virtual DateTimeOffset? AfterAt { get; set; }

        /// <inheritdoc />
        public virtual DateTimeOffset? BeforeAt { get; set; }

        /// <inheritdoc />
        public virtual IQueryable<T> GetQuery<T>(IQueryable<T> queryable)
            where T : IEntity
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (this.AfterAt.HasValue)
                queryable = queryable.Where(x => x.CreatedAt >= this.AfterAt.Value);

            if (this.BeforeAt.HasValue)
                queryable = queryable.Where(x => x.CreatedAt >= this.BeforeAt.Value);

            return queryable
                .Where(x => x.IsActive == this.IsActive);
        }
    }
}