using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Services.Interfaces
{
    /// <summary>
    /// (Base) interface for a spatial service.
    /// </summary>
    public interface IServiceSpatial : IService
    {
        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that covers the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that crosses the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that touches the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that overlaps the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that is covered by the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are disjointing of the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that intersects the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are within the radius of the <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteriaSpatial"/>.</typeparam>
        /// <param name="query">The <see cref="IQuery{TCriteria}"/>.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(IQuery<TCriteria> query, double distance = 10000D, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQueryCriteriaSpatial;
    }
}