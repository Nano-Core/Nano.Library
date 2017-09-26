using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Controllers.Contracts;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Services.Interfaces
{
    /// <summary>
    /// (Base) interface for a spatial service.
    /// </summary>
    public interface IServiceSpatial : IService
    {
        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that covers the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Covers<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that crosses the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Crosses<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that touches the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Touches<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that overlaps the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Overlaps<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that is covered by the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> CoveredBy<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are disjointing of the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Disjoints<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that intersects the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Intersects<TEntity>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s that are within the radius of the <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntitySpatial"/>.</typeparam>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/>.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of matching results.</returns>
        Task<IEnumerable<TEntity>> Within<TEntity>(CriteriaSpatialWithin criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntitySpatial;
    }
}