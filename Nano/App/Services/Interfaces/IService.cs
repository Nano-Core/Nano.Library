using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Controllers.Contracts;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;

namespace Nano.App.Services.Interfaces
{
    /// <summary>
    /// (Base) Interface for a service.
    /// Defines methods for the most rudamentory operations (get, query, add, update, delete) on instances of <see cref="IEntity"/>.
    /// <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/> and <see cref="IEntityDeletable"/>). 
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
        /// <param name="key"><see cref="object"/> to uniquely identify the <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
        Task<TEntity> Get<TEntity>(object key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all instances of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
        /// <param name="query">The <see cref="Query"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{TModel}"/> instance.</returns>
        Task<IEnumerable<TEntity>> GetAll<TEntity>(Query query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all instances of <typeparamref name="TEntity"/>, matching the criterias of the passed <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
        /// <typeparam name="TCriteria">The <see cref="ICriteria"/> type.</typeparam>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{TModel}"/> instance.</returns>
        Task<IEnumerable<TEntity>> GetMany<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, ICriteria, new();

        /// <summary>
        /// Get all instances of <typeparamref name="TEntity"/>, matching the passed <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
        /// <param name="expression">The <see cref="Expression"/> to evaulate entities to be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{TModel}"/> instance.</returns>
        Task<IEnumerable<TEntity>> GetMany<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;

        /// <summary>
        /// Adds the instance of the passed <see cref="IEntityCreatable"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
        /// <param name="entity">The instance of <see cref="IEntityCreatable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task Add<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable;

        /// <summary>
        /// Adds all instances of the passed <see cref="IEntityCreatable"/>'s.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
        /// <param name="entities">The instances of <see cref="IEntityCreatable"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task AddMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable;

        /// <summary>
        /// Updates the instance of the passed <see cref="IEntityUpdatable"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
        /// <param name="entity">The instance of <see cref="IEntityUpdatable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable;

        /// <summary>
        /// Updates all instances of the passed <see cref="IEntityDeletable"/>'s.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
        /// <param name="entities">The instances of <see cref="IEntityUpdatable"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task UpdateMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable;

        /// <summary>
        /// Adds or Updates the instance of the passed <see cref="IEntityCreatable"/> / <see cref="IEntityUpdatable"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
        /// <param name="entity">The instance of <see cref="IEntityUpdatable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task AddOrUpdate<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable, IEntityUpdatable;

        /// <summary>
        /// Deletes the instance of the passed <see cref="IEntityDeletable"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
        /// <param name="entity">The instance od <see cref="IEntityDeletable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task Delete<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable;

        /// <summary>
        /// Deletes all instances of the passed <see cref="IEntityDeletable"/>'s.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
        /// <param name="entities">The instances of <see cref="IEntityDeletable"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        Task DeleteMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable;
    }
}