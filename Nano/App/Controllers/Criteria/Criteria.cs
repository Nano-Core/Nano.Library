using Nano.App.Controllers.Criteria.Entities;
using Nano.App.Controllers.Criteria.Interfaces;

namespace Nano.App.Controllers.Criteria
{
    /// <inheritdoc />
    public class Criteria : ICriteria
    {
        /// <inheritdoc />
        public virtual Ordering Order { get; set; } = new Ordering();

        /// <inheritdoc />
        public virtual Pagination Paging { get; set; } = new Pagination();
    }

    /// <inheritdoc cref="ICriteria{TQuery}"/>
    public class Criteria<TQuery> : Criteria, ICriteria<TQuery>
        where TQuery : IQuery
    {
        /// <inheritdoc />
        public virtual TQuery Query { get; set; } 
    }
}