using System;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Criterias.Entities
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
        public virtual Filter GetExpression<TEntity>() 
            where TEntity : class, IEntity
        {
            var filter = new Filter();

            filter.Equal("IsActive", this.IsActive);

            if (this.BeforeAt.HasValue)
                filter.LessThanOrEqualTo("CreatedAt", this.BeforeAt);

            if (this.AfterAt.HasValue)
                filter.GreaterThanOrEqualTo("CreatedAt", this.AfterAt);

            return filter;
        }
    }
}