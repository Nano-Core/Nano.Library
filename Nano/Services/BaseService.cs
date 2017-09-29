using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.App.Data.Extensions;
using Nano.Controllers.Contracts;
using Nano.Controllers.Contracts.Extensions;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Services
{
    /// <summary>
    /// Base abstract implementation of <see cref="IService"/>.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="DbContext"/>.</typeparam>
    public class BaseService<TContext> : IService
        where TContext : DbContext
    {
        /// <summary>
        /// Data Context of type <typeparamref name="TContext"/>.
        /// </summary>
        public virtual TContext Context { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> of <typeparamref name="TContext"/>.</param>
        public BaseService(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
        }

        /// <inheritdoc />
        public virtual async Task<T> Get<T>(object key, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntity
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return await this.Context
                .Set<T>()
                .FindAsync(new[] { key }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAll<T>(Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntity
        {
            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetMany<T>(ICriteria criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntity
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetMany<T>(Expression<Func<T, bool>> expression, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntity
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(expression)
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Add<T>(T entity, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityCreatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .AddAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task AddMany<T>(IEnumerable<T> entities, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityCreatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .AddRangeAsync(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Update<T>(T entity, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .UpdateAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task UpdateMany<T>(IEnumerable<T> entities, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .UpdateRangeAsync(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Delete<T>(T entity, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityDeletable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await this.Context
                .RemoveAsync(entity, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteMany<T>(IEnumerable<T> entities, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntityDeletable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .RemoveRangeAsync(entities, cancellationToken);
          
                await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Dispose (non-virtual).
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool).
        /// Override in derived classes as needed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.Context?.Dispose();
        }
    }
}