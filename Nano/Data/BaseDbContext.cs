using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Data.Interfaces;
using Nano.Eventing.Attributes;
using Nano.Models.Events;
using Nano.Models.Interfaces;

namespace Nano.Data
{
    /// <inheritdoc cref="IDbContext"/>
    public abstract class BaseDbContext : DbContext, IDbContext
    {
        /// <inheritdoc />
        public virtual List<EntityEvent> EntityEvents { get; set; }

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
            return this.SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }

        /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)" />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc cref="DbContext.SaveChangesAsync(bool,CancellationToken)" />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.EntityEvents = this.ChangeTracker
                .Entries<IEntityIdentity<Guid>>()
                .Where(x => x.Entity.GetType().GetAttributes<PublishAttribute>().Any(y => y.States.Contains(x.State)))
                .Select(x => new EntityEvent
                {
                    Id = x.Entity.Id.ToString(),
                    State = x.State.ToString(),
                    Name = x.Entity.GetType().FullName
                })
                .ToList();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}