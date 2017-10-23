using Nano.App.Controllers.Criteria.Entities;

namespace Nano.App.Controllers.Criteria.Interfaces
{
    /// <summary>
    /// Query interface.
    /// </summary>
    public interface IQuery
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
    /// Query interface.
    /// </summary>
    /// <typeparam name="TCriteria">The type of <see cref="IQuery"/>.</typeparam>
    public interface IQuery<TCriteria>
        where TCriteria : ICriteria
    {
        /// <summary>
        /// Query.
        /// </summary>
        TCriteria Criteria { get; set; }
    }
}