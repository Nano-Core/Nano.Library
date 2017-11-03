using Nano.Controllers.Criterias.Interfaces;

namespace Nano.Controllers.Criterias.Entities
{
    /// <inheritdoc />
    public class Query : IQuery
    {
        /// <inheritdoc />
        public virtual Ordering Order { get; set; } = new Ordering();

        /// <inheritdoc />
        public virtual Pagination Paging { get; set; } = new Pagination();
    }

    /// <inheritdoc cref="IQuery{TCriteria}"/>
    public class Query<TCriteria> : Query, IQuery<TCriteria>
        where TCriteria : ICriteria
    {
        /// <inheritdoc />
        public virtual TCriteria Criteria { get; set; }
    }
}