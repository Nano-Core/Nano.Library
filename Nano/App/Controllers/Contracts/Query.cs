using Nano.App.Controllers.Contracts.Interfaces;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Order.
        /// </summary>
        public Ordering Order { get; set; } = new Ordering();

        /// <summary>
        /// Paging.
        /// </summary>
        public Pagination Paging { get; set; } = new Pagination();
    }

    /// <summary>
    /// Query.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="BaseCriteria"/>.</typeparam>
    public class Query<TEntity> : Query
        where TEntity : ICriteria, new()
    {
        /// <summary>
        /// Criteria.
        /// </summary>
        public TEntity Criteria { get; set; } = new TEntity();
    }
}