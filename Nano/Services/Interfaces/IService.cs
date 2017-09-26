using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nano.Controllers.Contracts;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Services.Interfaces
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
        /// <typeparam name="TEntity">A type implementing <see cref="IEntity"/>.</typeparam>
        /// <param name="key"><see cref="object"/> to uniquely identify the <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
        Task<TEntity> Get<TEntity>(object key, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all instances of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntity"/>.</typeparam>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{TModel}"/> instance.</returns>
        Task<IEnumerable<TEntity>> GetAll<TEntity>(Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntity;

        /// <summary>
        /// Get all instances of <typeparamref name="TEntity"/>, matching the criteria of the passed <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntity"/>.</typeparam>
        /// <param name="expression">The <see cref="Expression"/> to evaulate entities to be returned.</param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="IEnumerable{TModel}"/> instance.</returns>
        Task<IEnumerable<TEntity>> GetMany<TEntity>(Expression<Func<TEntity, bool>> expression, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="criteria"></param>
        /// <param name="paging"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetMany<TEntity>(ICriteria criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntity;

        /// <summary>
        /// Adds the instance of the passed <paramref name="entity"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityCreatable"/>.</typeparam>
        /// <param name="entity">The <see cref="IEntityCreatable"/> instance to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task Add<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityCreatable;

        /// <summary>
        /// Adds all instances of the passed <paramref name="entities"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityCreatable"/>.</typeparam>
        /// <param name="entities">The <see cref="IEnumerable{T}"/> of <see cref="IEntityCreatable"/> instances to add.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task AddMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityCreatable;

        /// <summary>
        /// Updates the instance of the passed <paramref name="entity"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityUpdatable"/>.</typeparam>
        /// <param name="entity">The <see cref="IEntityUpdatable"/> instance to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task Update<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityUpdatable;

        /// <summary>
        /// Updates all instances of the passed <paramref name="entities"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityUpdatable"/>.</typeparam>
        /// <param name="entities">The <see cref="IEnumerable{T}"/> of <see cref="IEntityUpdatable"/> instances to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task UpdateMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityUpdatable;

        /// <summary>
        /// Deletes the instance of the passed <paramref name="entity"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityDeletable"/>.</typeparam>
        /// <param name="entity">The <see cref="IEntityDeletable"/> instance to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task Delete<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityDeletable;

        /// <summary>
        /// Deletes all instances of the passed <paramref name="entities"/> parameter.
        /// </summary>
        /// <typeparam name="TEntity">A <see cref="Type"/> implementing <see cref="IEntityDeletable"/>.</typeparam>
        /// <param name="entities">The <see cref="IEnumerable{T}"/> of <see cref="IEntityDeletable"/> instances to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A <see cref="Task"/> returning 'void'.</returns>
        Task DeleteMany<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
            where TEntity : class, IEntityDeletable;
    }
}