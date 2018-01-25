using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Data.Models;
using Nano.Data.Models.Mappings;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Models.Interfaces;
using Newtonsoft.Json;
using Z.EntityFramework.Plus;

namespace Nano.Data
{
    /// <inheritdoc />
    public abstract class BaseDbContext : DbContext
    {
        /// <summary>
        /// Options.
        /// </summary>
        public virtual DataOptions Options { get; set; }

        /// <summary>
        /// Audit Entries.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public virtual DbSet<AuditEntry> __EFAudit { get; set; }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Audit Entry Properties.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public virtual DbSet<AuditEntryProperty> __EFAuditProperties { get; set; }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextOptions">The <see cref="DbContextOptions"/>.</param>
        /// <param name="dataOptions">The <see cref="DataOptions"/>.</param>
        protected BaseDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
            : base(contextOptions)
        {
            if (dataOptions == null)
                throw new ArgumentNullException(nameof(dataOptions));

            this.Options = dataOptions;
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder
                .AddMapping<DefaultAuditEntry, DefaultAuditEntryMapping>();
        }

        /// <summary>
        /// Import data from the passed <paramref name="uri"/>, deserilaized into the type of the generic argument <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type, used for deserialization.</typeparam>
        /// <param name="uri">The <see cref="Uri"/> of the data to import.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        public virtual async Task AddRangeAsync<TEntity>(Uri uri, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            await this.AddRangeAsync(uri, typeof(TEntity), cancellationToken);
        }

        /// <summary>
        /// Import data from the passed <paramref name="uri"/>, deserilaized into the passed <paramref name="type"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the data to import.</param>
        /// <param name="type">The <see cref="Type"/> used when deserializing.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        public virtual async Task AddRangeAsync(Uri uri, Type type, CancellationToken cancellationToken = default)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            using (var httpClient = new HttpClient())
            {
                await httpClient
                    .GetAsync(uri, cancellationToken)
                    .ContinueWith(async x =>
                    {
                        var result = await x;
                        var json = await result.Content.ReadAsStringAsync();
                        var entities = JsonConvert.DeserializeObject(json, type);

                        await this.AddRangeAsync(entities, cancellationToken);
                    }, cancellationToken);
            }
        }

        /// <summary>
        /// Adds or updates (if exists) the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <returns>A <see cref="EntityEntry{TEntity}"/>.</returns>
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

        /// <summary>
        /// Adds or updates (if exists) the entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entities"/>.</typeparam>
        /// <param name="entities">The <see cref="object"/>'s of type <typeparamref name="TEntity"/>.</param>
        /// <returns>A <see cref="EntityEntry{TEntity}"/>.</returns>
        public virtual void AddOrUpdateMany<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                this.AddOrUpdate(entity);
            }
        }
    }
}