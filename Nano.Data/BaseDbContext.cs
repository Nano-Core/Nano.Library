using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Extensions;
using Nano.Data.Mappings;
using Nano.Data.Mappings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Nano.Data;

/// <inheritdoc />
public class BaseDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions)
    : BaseDbContext<Guid>(contextOptions, dataOptions);

/// <summary>
/// Base DbContext for identity and application data, with support for auditing, soft deletion, and entity events.
/// </summary>
/// <typeparam name="TIdentity">The type used for the identity (e.g., Guid, int, string).</typeparam>
public abstract class BaseDbContext<TIdentity> : IdentityDbContext<IdentityUserEx<TIdentity>, IdentityRole<TIdentity>, TIdentity, IdentityUserClaim<TIdentity>, IdentityUserRole<TIdentity>, IdentityUserLogin<TIdentity>, IdentityRoleClaim<TIdentity>, IdentityUserToken<TIdentity>>, IDataProtectionKeyContext
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IOptionsMonitor<DataOptions> options;
    private readonly EntityGraphHydrator graphHydrator;

    /// <summary>
    /// Gets or sets the DbSet for data protection keys.
    /// </summary>
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDbContext{TIdentity}"/> class.
    /// </summary>
    /// <param name="contextOptions">The <see cref="DbContextOptions"/>.</param>
    /// <param name="options">The <see cref="DataOptions"/> monitor.</param>
    protected BaseDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> options)
        : base(contextOptions)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.graphHydrator = new EntityGraphHydrator(this);
    }

    /// <inheritdoc />
    public override EntityEntry Update(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = base.Update(entity);

        this.ProcessUpdatedEntry(entry);

        return entry;
    }

    /// <inheritdoc />
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = base.Update(entity);

        this.ProcessUpdatedEntry(entry);

        return entry;
    }

    /// <inheritdoc />
    public override void UpdateRange(params object[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entries = entities
            .Select(this.Entry)
            .ToArray();

        base.UpdateRange(entities);

        foreach (var entry in entries)
        {
            this.ProcessUpdatedEntry(entry);
        }
    }

    /// <inheritdoc />
    public override void UpdateRange(IEnumerable<object> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var entitiesArray = entities
            .ToArray();

        var entries = entitiesArray
            .Select(this.Entry);

        base.UpdateRange(entitiesArray);

        foreach (var entry in entries)
        {
            this.ProcessUpdatedEntry(entry);
        }
    }

    /// <summary>
    /// Adds a new entity or updates it if it already exists in the context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add or update.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}"/> for the entity.</returns>
    public virtual EntityEntry<TEntity> AddOrUpdate<TEntity>(TEntity entity)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var tracked = this.ChangeTracker
            .Entries<TEntity>()
            .FirstOrDefault(x => x.Entity == entity);

        if (tracked != null)
        {
            tracked.CurrentValues
                .SetValues(entity);

            return tracked;
        }

        var keyProperties = this.Model
            .FindEntityType(typeof(TEntity))?
            .FindPrimaryKey()?
            .Properties;

        if (keyProperties == null || keyProperties.Count == 0)
        {
            throw new InvalidOperationException($"No primary key defined for '{typeof(TEntity).Name}'");
        }

        var keyValues = keyProperties
            .Select(x => x.PropertyInfo?
                .GetValue(entity))
            .ToArray();

        var existing = this.Set<TEntity>()
            .Find(keyValues);

        if (existing != null)
        {
            this.Entry(existing).CurrentValues
                .SetValues(entity);

            return this.Entry(existing);
        }

        return this.Add(entity);
    }

    /// <summary>
    /// Adds or updates multiple entities in the context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <param name="entities">The entities to add or update.</param>
    public virtual void AddOrUpdateMany<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            this.AddOrUpdate(entity);
        }
    }

    /// <summary>
    /// Saves all changes made in the context to the database with auditing and entity event support.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        var audit = new Audit();

        audit
            .PreSaveChanges(this);

        var rowAffecteds = this.SaveChangesWithTriggers(base.SaveChanges);

        audit
            .PostSaveChanges();

        if (audit.Entries.Count > 0)
        {
            AuditManager.DefaultConfiguration.AutoSavePreAction
                .Invoke(this, audit);

            base.SaveChanges();
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Saves all changes made in the context to the database with auditing and entity event support.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have been sent successfully to the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var audit = new Audit();

        audit
            .PreSaveChanges(this);

        var rowAffecteds = this.SaveChangesWithTriggers(x => base.SaveChanges(x), acceptAllChangesOnSuccess);

        audit
            .PostSaveChanges();

        if (audit.Entries.Count > 0)
        {
            AuditManager.DefaultConfiguration.AutoSavePreAction
                .Invoke(this, audit);

            base.SaveChanges(acceptAllChangesOnSuccess);
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Asynchronously saves all changes made in the context to the database with auditing and entity event support.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> for cancelling the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var audit = new Audit();

        audit
            .PreSaveChanges(this);

        var rowAffecteds = await this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, cancellationToken);

        audit
            .PostSaveChanges();

        if (audit.Entries.Count > 0)
        {
            AuditManager.DefaultConfiguration.AutoSavePreAction
                .Invoke(this, audit);

            await base.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Asynchronously saves all changes made in the context to the database with auditing and entity event support.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have been sent successfully to the database.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> for cancelling the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var audit = new Audit();

        audit
            .PreSaveChanges(this);

        var rowAffecteds = await this.SaveChangesWithTriggersAsync((x, innerCancellationToken) => base.SaveChangesAsync(x, innerCancellationToken), acceptAllChangesOnSuccess, cancellationToken);

        audit
            .PostSaveChanges();

        if (audit.Entries.Count > 0)
        {
            AuditManager.DefaultConfiguration.AutoSavePreAction
                .Invoke(this, audit);

            await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                .ConfigureAwait(false);
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Configures the model for the context including identity mapping, auditing, and default collation.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrEmpty(this.options.CurrentValue.DefaultCollation))
        {
            modelBuilder
                .UseCollation(this.options.CurrentValue.DefaultCollation);
        }

        modelBuilder
            .MapEntities<TIdentity>()
            .MapIdentityEntities<TIdentity>(this.options.CurrentValue.Identity)
            .AddMapping<AuditEntry<TIdentity>, TIdentity, AuditEntryMapping<TIdentity>>()
            .AddMapping<AuditEntryProperty<TIdentity>, TIdentity, AuditEntryPropertyMapping<TIdentity>>();
    }


    private void ProcessUpdatedEntry(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        this.graphHydrator
            .HydrateAudit(entry, visited);
    }
}