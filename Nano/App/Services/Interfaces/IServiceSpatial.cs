using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Controllers.Criteria;
using Nano.App.Controllers.Criteria.Interfaces;
using Nano.App.Models.Interfaces;

namespace Nano.App.Services.Interfaces
{
    /// <summary>
    /// (Base) interface for a spatial service.
    /// </summary>
    public interface IServiceSpatial : IService
    {
        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that covers the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Covers<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that crosses the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Crosses<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that touches the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Touches<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that overlaps the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Overlaps<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that is covered by the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> CoveredBy<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are disjointing of the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Disjoints<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that intersects the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Intersects<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are within the radius of the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TQuery">The type of <see cref="IQuerySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="Criteria{TQuery}"/>.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Within<TEntity, TQuery>(Criteria<TQuery> criteria, double distance = 10000D, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial;
    }
}