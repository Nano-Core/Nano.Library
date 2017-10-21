using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.App.Models;
using Nano.Data.Interfaces;
using Nano.Eventing;
using Nano.Eventing.Attributes;
using Nano.Eventing.Providers.Interfaces;

namespace Nano.Data
{
    /// <inheritdoc cref="IDbContext"/>
    public abstract class BaseDbContext : DbContext, IDbContext
    {
        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventingProvider Eventing => this.GetService<IEventingProvider>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="DbContextOptions"/>.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc />
        public virtual Task<EntityEntry<TEntity>> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Update(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() =>
                {
                    foreach (var entity in entities)
                    {
                        this.Update(entity);
                    }
                }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<EntityEntry<TEntity>> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Remove(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task RemoveRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() =>
                {
                    foreach (var entity in entities)
                    {
                        this.Remove(entity);
                    }
                }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual EntityEntry<TEntity> AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var dbSet = this.Set<TEntity>();
            var tracked = dbSet.SingleOrDefault(x => x == entity);

            if (tracked != null)
            {
                this.Entry(tracked).CurrentValues.SetValues(entity);
                return this.Entry(tracked);
            }

            return dbSet.Add(entity);
        }

        /// <inheritdoc />
        public virtual Task<EntityEntry<TEntity>> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.AddOrUpdate(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual void AddOrUpdateMany(IEnumerable<object> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                this.AddOrUpdate(entity);
            }
        }

        /// <inheritdoc />
        public virtual Task AddOrUpdateManyAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() => { this.AddOrUpdateMany(entities); }, cancellationToken);
        }

        /// <inheritdoc cref="DbContext.SaveChanges()" />
        public override int SaveChanges()
        {
            return this.SaveChanges(true);
        }

        /// <inheritdoc cref="DbContext.SaveChanges(bool)" />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var entries = this.GetChanges();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);

            this.PublishChanges(entries);

            return result;
        }

        /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)" />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc cref="DbContext.SaveChangesAsync(bool,CancellationToken)" />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = this.GetChanges();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                .ContinueWith(x =>
                {
                    this.PublishChanges(entries);

                    return x.Result;
                }, cancellationToken);
        }

        // TODO: Refactor, dont use DefaultEntity!
        private EntityEntry<DefaultEntity>[] GetChanges()
        {
            return this.ChangeTracker
                .Entries<DefaultEntity>()
                .Where(y => y.State == EntityState.Added || y.State == EntityState.Modified || y.State == EntityState.Deleted)
                .ToArray();
        }
        private void PublishChanges(IEnumerable<EntityEntry<DefaultEntity>> entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                var type = entry.Entity.GetType();
                var attributes = type.GetAttributes<EventingAttribute>().ToArray();

                if (attributes.Any())
                {
                    this.Eventing.PublishAsync(new EventEntry
                    {
                        Id = entity.Id,
                        Type = type.FullName,
                        State = entry.State
                    });
                }
            }
        }
    }
}