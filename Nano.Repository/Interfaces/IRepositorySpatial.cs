using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.Models.Criterias;
using Nano.Models.Interfaces;

namespace Nano.Repository.Interfaces;

/// <summary>
/// (Base) interface for a spatial repository.
/// </summary>
public interface IRepositorySpatial : IRepository
{
    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that covers the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that covers the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that crosses the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that crosses the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that touches the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that touches the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that overlaps the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that overlaps the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that is covered by the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that is covered by the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that are disjointing of the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that are disjointing of the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that intersects the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(SpatialQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that intersects the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that are within the radius of the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="distance">The distance in meters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(SpatialQuery<TCriteria> query, double distance = 10000D, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets <see cref="IEntitySpatial"/>'s that are within the radius of the <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
    /// <typeparam name="TCriteria">The type of <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="query">The <see cref="SpatialQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="distance">The distance in meters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
    Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(SpatialQuery<TCriteria> query, int includeDepth, double distance = 10000D, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteria, new();
}