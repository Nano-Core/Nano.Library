using Nano.App.Controllers.Criteria.Entities;

namespace Nano.App.Controllers.Criteria.Interfaces
{
    /// <summary>
    /// Criteria interface.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Order.
        /// </summary>
        Ordering Order { get; set; }

        /// <summary>
        /// Paging.
        /// </summary>
        Pagination Paging { get; set; }
    }

    /// <summary>
    /// Criteria interface.
    /// </summary>
    /// <typeparam name="TQuery">The type of <see cref="IQuery"/>.</typeparam>
    public interface ICriteria<TQuery>
        where TQuery : IQuery
    {
        /// <summary>
        /// Query.
        /// </summary>
        TQuery Query { get; set; }
    }
}