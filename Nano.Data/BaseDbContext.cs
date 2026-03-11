using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Eventing.Extensions;
using Nano.Data.Extensions;
using Nano.Data.Identity.Extensions;
using Nano.Data.Mappings;
using Nano.Data.Mappings.Extensions;
using Nano.Eventing.Abstractions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Nano.Data;

// BUG: Entity Event Map (Important)
// 1. Make a map of Publish Attributes and their property names.
// 2. When SaveChanges then check if any property names are affected (e.g. User.IdentityUser.Email is changed, then User needs to be fetched and published)
// NB: Maybe make startup validation of Publish properties and use of include

/// <inheritdoc />
public class BaseDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> dataOptions, IEventing? eventing = null)
    : BaseDbContext<Guid>(contextOptions, dataOptions, eventing);

/// <summary>
/// Base DbContext for identity and application data, with support for auditing, soft deletion, and entity events.
/// </summary>
/// <typeparam name="TIdentity">The type used for the identity (e.g., Guid, int, string).</typeparam>
public abstract class BaseDbContext<TIdentity> : IdentityDbContext<IdentityUserEx<TIdentity>, IdentityRole<TIdentity>, TIdentity, IdentityUserClaim<TIdentity>, IdentityUserRole<TIdentity>, IdentityUserLogin<TIdentity>, IdentityRoleClaim<TIdentity>, IdentityUserToken<TIdentity>>, IDataProtectionKeyContext
    where TIdentity : IEquatable<TIdentity>
{
    private bool isEntityEventEnabled = true;
    private ConcurrentQueue<EntityEvent> pendingEvents = [];

    private readonly IOptionsMonitor<DataOptions> options;
    private readonly IEventing? eventing;

    /// <summary>
    /// Gets or sets the DbSet for data protection keys.
    /// </summary>
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseDbContext{TIdentity}"/> class.
    /// </summary>
    /// <param name="contextOptions">The <see cref="DbContextOptions"/>.</param>
    /// <param name="options">The <see cref="DataOptions"/> monitor.</param>
    /// <param name="eventing">Optional eventing service for publishing entity events.</param>
    protected BaseDbContext(DbContextOptions contextOptions, IOptionsMonitor<DataOptions> options, IEventing? eventing = null)
        : base(contextOptions)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.eventing = eventing;

        this.SavingChanges += (_, _) => this.PreSaveEntityEvents();
        this.SavingChanges += (_, _) => this.UpdateSoftDeletedEntities();
        this.SavedChanges += async (_, _) => await this.PublishEntityEvents();

        // ReSharper disable VirtualMemberCallInConstructor
        this.ChangeTracker.LazyLoadingEnabled = this.options.CurrentValue.UseLazyLoading;
        // ReSharper restore VirtualMemberCallInConstructor
    }

    /// <summary>
    /// Updates an entity in the context.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The <see cref="EntityEntry"/> representing the entity.</returns>
    public override EntityEntry Update(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var isAuditEnabled = this.options.CurrentValue.UseAudit;

        var publishAnnotation = entity
            .GetType()
            .GetCustomAttribute<PublishAttribute>(true);

        var hasPublishProperties = publishAnnotation != null && publishAnnotation.PropertyNames.Any();

        if (!isAuditEnabled && !hasPublishProperties)
        {
            return base.Update(entity);
        }

        var existingEntry = this.ChangeTracker
            .Entries()
            .FirstOrDefault(x => x.Entity == entity);

        if (existingEntry != null)
        {
            return base.Update(entity);
        }

        var dbSet = this.SetDynamic(entity.GetType().Name);

        var tracked = dbSet
            .AsNoTracking()
            .SingleOrDefault(x => x == entity);

        this.SetOriginalValues(entity, tracked);

        return base.Update(entity);
    }

    /// <summary>
    /// Updates a typed entity in the context.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}"/> representing the entity.</returns>
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var isAuditEnabled = this.options.CurrentValue.UseAudit;

        var publishAnnotation = entity
            .GetType()
            .GetCustomAttribute<PublishAttribute>(true);

        var hasPublishProperties = publishAnnotation != null && publishAnnotation.PropertyNames.Any();

        if (!isAuditEnabled && !hasPublishProperties)
        {
            return base.Update(entity);
        }

        var existingEntry = this.ChangeTracker
            .Entries()
            .FirstOrDefault(x => x.Entity == entity);

        if (existingEntry != null)
        {
            return base.Update(entity);
        }

        var dbSet = this.Set<TEntity>();

        var tracked = dbSet
            .AsNoTracking()
            .SingleOrDefault(x => x == entity);

        this.SetOriginalValues(entity, tracked);

        return base.Update(entity);
    }

    /// <summary>
    /// Updates a range of entities in the context.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    public override void UpdateRange(params object[] entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var isAuditEnabled = this.options.CurrentValue.UseAudit;

        var firstEntity = entities
            .First();

        var publishAnnotation = firstEntity
            .GetType()
            .GetCustomAttribute<PublishAttribute>(true);

        var hasPublishProperties = publishAnnotation != null && publishAnnotation.PropertyNames.Any();

        if (isAuditEnabled || hasPublishProperties)
        {
            var nonExistingEntries = this.ChangeTracker
                .Entries()
                .Where(x => entities.Any(y => y != x.Entity))
                .ToArray();

            if (nonExistingEntries.Any())
            {
                var dbSet = this.SetDynamic(firstEntity.GetType().Name);

                var trackeds = dbSet
                    .AsNoTracking()
                    .Where(x => nonExistingEntries
                        .Any(y => x == y.Entity));

                foreach (var entity in trackeds)
                {
                    var tracked = trackeds
                        .SingleOrDefault(x => x == entity);

                    this.SetOriginalValues(entity, tracked);
                }
            }
        }
        foreach (var entity in entities)
        {
            this.Update(entity);
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
            this.Entry(tracked).CurrentValues.SetValues(entity);

            return tracked;
        }

        var idProperty = entity
            .GetType()
            .GetProperty(nameof(IEntityIdentity<>.Id));

        if (idProperty == null)
        {
            throw new NullReferenceException(nameof(idProperty));
        }

        var id = idProperty.GetValue(entity);

        var existing = this.Set<TEntity>()
            .Find(id);

        // BUG: reflection id
        //var existing = this.Set<TEntity>()
        //    .Find(entity.Id);

        if (existing != null)
        {
            this.Entry(existing).CurrentValues.SetValues(entity);
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
        where TEntity : class, IEntity // IEntityIdentity<TIdentity>
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

        var rowAffecteds = this.SaveChangesWithTriggers(this.SaveChanges);

        audit
            .PostSaveChanges();

        var autoSavePreAction = audit.Configuration.AutoSavePreAction ?? AuditManager.DefaultConfiguration.AutoSavePreAction;

        if (autoSavePreAction == null || audit.Entries.Count == 0)
        {
            return rowAffecteds;
        }

        autoSavePreAction
            .Invoke(this, audit);

        this.SaveChanges();

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

        var rowAffecteds = await this.SaveChangesWithTriggersAsync(this.SaveChangesAsync, cancellationToken)
            .ConfigureAwait(false);

        audit
            .PostSaveChanges();

        var autoSavePreAction = audit.Configuration.AutoSavePreAction ?? AuditManager.DefaultConfiguration.AutoSavePreAction;

        if (autoSavePreAction == null || audit.Entries.Count == 0)
        {
            return rowAffecteds;
        }

        autoSavePreAction
            .Invoke(this, audit);

        await this.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        return rowAffecteds;
    }

    internal virtual async Task<int> SaveChangesWithoutEntityEventsAsync(CancellationToken cancellationToken = default)
    {
        this.isEntityEventEnabled = false;

        try
        {
            return await this.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            this.isEntityEventEnabled = true;
        }
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


    private void PreSaveEntityEvents()
    {
        if (this.eventing == null || !this.isEntityEventEnabled)
        {
            return;
        }

        var entityEvents = this.ChangeTracker
            .GetPendingEntityEvents();

        foreach (var entityEvent in entityEvents)
        {
            this.pendingEvents
                .Enqueue(entityEvent);
        }
    }
    private async Task PublishEntityEvents(CancellationToken cancellationToken = default)
    {
        if (this.eventing == null || !this.isEntityEventEnabled)
        {
            return;
        }

        try
        {
            while (!this.pendingEvents.IsEmpty)
            {
                var success = this.pendingEvents
                    .TryDequeue(out var entityEvent);

                if (success && entityEvent != null)
                {
                    await this.eventing
                        .PublishAsync(entityEvent, entityEvent.Type, cancellationToken);
                }
            }
        }
        finally
        {
            this.pendingEvents
                .Clear();

            this.pendingEvents = [];
        }
    }
    private void UpdateSoftDeletedEntities()
    {
        if (!this.options.CurrentValue.UseSoftDeletetion)
        {
            return;
        }

        var entityEntries = this.ChangeTracker
            .Entries<IEntityDeletableSoft>()
            .Where(x => x.State == EntityState.Deleted);

        foreach (var entityEntry in entityEntries)
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Entity.IsDeleted = DateTimeOffset.UtcNow.GetEpochTime();
        }
    }
    private void SetOriginalValues(object entity, object? tracked = null, EntityEntry? owner = null, string? propertName = null)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (tracked == null)
        {
            return;
        }

        this.ChangeTracker.LazyLoadingEnabled = false;

        try
        {
            var entry = owner == null
                ? this.Entry(entity)
                : propertName == null
                    ? this.Entry(entity)
                    : owner.Reference(propertName).TargetEntry;

            if (entry == null)
            {
                return;
            }

            var properties = entity
                .GetType()
                .GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.SetMethod == null)
                {
                    continue;
                }

                var hasNotMappedAttribute = propertyInfo
                    .GetCustomAttribute<NotMappedAttribute>();

                if (hasNotMappedAttribute != null)
                {
                    continue;
                }

                var valueTracked = propertyInfo
                    .GetValue(tracked);

                if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    entry
                        .Property(propertyInfo.Name).OriginalValue = valueTracked;
                }
                else if (!propertyInfo.PropertyType.IsTypeOf(typeof(IEnumerable)) && !propertyInfo.PropertyType.IsTypeOf(typeof(IEntity)))
                {
                    var value = propertyInfo
                        .GetValue(entity);

                    if (value == null)
                    {
                        continue;
                    }

                    this.SetOriginalValues(value, valueTracked, entry, propertyInfo.Name);
                }
            }
        }
        finally
        {
            if (this.options.CurrentValue.UseLazyLoading)
            {
                this.ChangeTracker.LazyLoadingEnabled = true;
            }
        }
    }
}