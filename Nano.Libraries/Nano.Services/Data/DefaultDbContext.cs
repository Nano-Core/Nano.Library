using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.Data;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
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
            var audit = new Audit();
            audit.PreSaveChanges(this);
            audit.Configuration.AutoSavePreAction?.Invoke(this, audit);
            audit.PostSaveChanges();

            this.ChangeTracker
                .Entries<IEntityDeletableSoft>()
                .Where(x => x.State == EntityState.Deleted)
                .ToList()
                .ForEach(x =>
                {
                    x.Entity.IsActive = false;
                    x.State = EntityState.Modified;
                });

            var entities = this.ChangeTracker
                .Entries<IEntity>()
                .Where(x => x.Entity.GetType().GetAttributes<PublishAttribute>().Any())
                .Select(x => x.Entity);

            return await base
                .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted || x.IsCanceled)
                        return x;

                    var eventing = this.GetService<IEventing>();

                    if (eventing == null)
                        return x;

                    foreach (var entity in entities)
                    {
                        var key = entity.GetType().Name;
                        var entityEvent = new EntityEvent(entity);

                        eventing
                            .Publish(entityEvent, key)
                            .ConfigureAwait(false);
                    }

                    return x;
                }, cancellationToken)
                .Result;
        }
    }
}