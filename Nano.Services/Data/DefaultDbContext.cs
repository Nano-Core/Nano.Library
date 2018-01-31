using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.Data;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Services.Eventing;
using Z.EntityFramework.Plus;

namespace Nano.Services.Data
{
    /// <inheritdoc />
    public class DefaultDbContext : BaseDbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextOptions">The <see cref="DbContextOptions"/>.</param>
        /// <param name="dataOptions">The <see cref="DataOptions"/>.</param>
        public DefaultDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
            : base(contextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.OnModelCreating(builder);
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            return this.SaveChanges(true);
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return this.SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.SaveAudit();
            this.SoftDelete();

            // TODO: Move to BaseService
            var entityEntries = this.ChangeTracker
                .Entries()
                .Where(x => 
                    x.Entity.GetType().IsTypeDef(typeof(IEntityIdentity<>)) &&
                    x.Entity.GetType().GetCustomAttributes<PublishAttribute>().Any())
                .Cast<EntityEntry<IEntityIdentity<Guid>>>(); // BUG: Remove cast, but method 'PublishAsync' needs it as parameter

            return await base
                .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                .ContinueWith(x =>
                {
                    if (!x.IsFaulted && !x.IsCanceled)
                    {
                        this.PublishAsync(entityEntries);
                    }

                    return x;
                }, cancellationToken)
                .Result;
        }

        private void SaveAudit()
        {
            var audit = new Audit();
            audit.PreSaveChanges(this);
            audit.Configuration.AutoSavePreAction?.Invoke(this, audit);
            audit.PostSaveChanges();
        }
        private void SoftDelete()
        {
            this.ChangeTracker
                .Entries<IEntityDeletableSoft>()
                .Where(x => x.State == EntityState.Deleted)
                .ToList()
                .ForEach(x =>
                {
                    x.Entity.IsActive = false;
                    x.State = EntityState.Modified;
                });

        }
        private void PublishAsync(IEnumerable<EntityEntry<IEntityIdentity<Guid>>> entityEntries)
        {
            if (entityEntries == null)
                throw new ArgumentNullException(nameof(entityEntries));

            var eventing = this.GetService<IEventing>();

            if (eventing == null)
                return;

            foreach (var entityEntry in entityEntries)
            {
                var key = entityEntry.GetType().FullName; // BUG: Eventing Annotation: Consider routing key. Add Application name?
                var entityEvent = new EntityEvent(entityEntry);

                eventing
                    .PublishAsync(entityEvent, key)
                    .ConfigureAwait(false);
            }
        }
    }
}