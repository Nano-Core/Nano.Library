using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Nano.Config;
using Nano.Data.Extensions;
using Nano.Data.Models;
using Nano.Data.Models.Mappings;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Models.Attributes;
using Nano.Models.Data;
using Nano.Models.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Security;
using Nano.Security.Const;
using Z.EntityFramework.Plus;

namespace Nano.Data;

/// <summary>
/// Base Db Context (abstract).
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseDbContext<TIdentity> : IdentityDbContext<IdentityUser<TIdentity>, IdentityRole<TIdentity>, TIdentity, IdentityUserClaim<TIdentity>, IdentityUserRole<TIdentity>, IdentityUserLogin<TIdentity>, IdentityRoleClaim<TIdentity>, IdentityUserTokenExpiry<TIdentity>>, IDataProtectionKeyContext
    where TIdentity : IEquatable<TIdentity>
{
    private IList<EntityEvent> pendingEvents = new List<EntityEvent>();

    /// <summary>
    /// Options.
    /// </summary>
    public DataOptions Options { get; }

    /// <summary>
    /// Auto Save.
    /// </summary>
    public virtual bool AutoSave => this.Options.UseAutoSave;

    /// <summary>
    /// Auto Save.
    /// </summary>
    public virtual bool IsEntityEventEnabled { get; set; } = true;

    /// <summary>
    /// Data Protection Keys.
    /// Required by <see cref="IDataProtectionKeyContext"/>.
    /// </summary>
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    /// <inheritdoc />
    protected BaseDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
        : base(contextOptions)
    {
        this.Options = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));

        this.SavingChanges += (_, _) => this.SetPendingEntityEvents();
        this.SavingChanges += (_, _) => this.UpdateSoftDeletedEntities();
        this.SavedChanges += async (_, _) => await this.PublishEntityEvents();

        // ReSharper disable VirtualMemberCallInConstructor
        this.ChangeTracker.LazyLoadingEnabled = this.Options.UseLazyLoading;
        // ReSharper restore VirtualMemberCallInConstructor
    }

    /// <inheritdoc />
    public override EntityEntry Update(object entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingEntry = this.ChangeTracker
            .Entries()
            .FirstOrDefault(x => x.Entity == entity);

        if (existingEntry == null)
        {
            var dbSet = this.SetDynamic(entity.GetType().Name);

            var tracked = dbSet
                .AsNoTracking()
                .SingleOrDefault(x => x == entity);

            this.UpdateOriginalValues(entity, tracked);
        }

        return base.Update(entity);
    }

    /// <inheritdoc />
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existingEntry = ChangeTracker.Entries()
            .FirstOrDefault(x => x.Entity == entity);

        if (existingEntry == null)
        {
            var dbSet = this.Set<TEntity>();

            var tracked = dbSet
                .AsNoTracking()
                .SingleOrDefault(x => x == entity);

            this.UpdateOriginalValues(entity, tracked);
        }

        return base.Update(entity);
    }

    /// <inheritdoc />
    public override void UpdateRange(params object[] entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            this.Update(entity);
        }
    }

    /// <inheritdoc />
    public override void UpdateRange(IEnumerable<object> entities)
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            this.Update(entity);
        }
    }

    /// <summary>
    /// Adds or updates (if exists) the entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
    /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
    /// <returns>A <see cref="EntityEntry"/>.</returns>
    public virtual EntityEntry<TEntity> AddOrUpdate<TEntity>(TEntity entity)
        where TEntity : class
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var tracked = this.ChangeTracker
            .Entries<TEntity>()
            .FirstOrDefault(x => x.Entity == entity)?.Entity;

        if (tracked == null)
        {
            var dbSet = this.Set<TEntity>();

            tracked = dbSet
                .FirstOrDefault(x => x == entity);
        }

        if (tracked != null)
        {
            return this
                .Update(entity);
        }

        return this
            .Add(entity);
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

    /// <inheritdoc />
    public override int SaveChanges()
    {
        var audit = new Audit();

        audit
            .PreSaveChanges(this);

        var rowAffecteds = this.SaveChangesWithTriggers(this.SaveChanges);

        audit
            .PostSaveChanges();

        var autoSavePreAction = audit.Configuration.AutoSavePreAction ?? AuditManager.DefaultConfiguration.AutoSavePreAction;

        if (autoSavePreAction != null)
        {
            if (audit.Entries.Any())
            {
                autoSavePreAction
                    .Invoke(this, audit);

                this.SaveChanges();
            }
        }

        return rowAffecteds;
    }

    /// <inheritdoc />
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

        if (autoSavePreAction != null)
        {
            if (audit.Entries.Any())
            {
                autoSavePreAction
                    .Invoke(this, audit);

                await this.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Create database.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    internal virtual async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        if (!ConfigManager.HasDbContext)
            return;

        if (!this.Options.UseCreateDatabase)
            return;

        if (this.Options.ConnectionString == null)
            return;

        await this.Database
            .EnsureCreatedAsync(cancellationToken);
    }

    /// <summary>
    /// Migrate database.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    internal virtual Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
    {
        if (!ConfigManager.HasDbContext)
            return Task.CompletedTask;

        if (!this.Options.UseMigrateDatabase)
            return Task.CompletedTask;

        if (this.Options.ConnectionString == null)
            return Task.CompletedTask;

        var logger = this.GetService<ILogger>();

        logger
            .LogInformation("Applying Migrations at start-up.");

        return this.Database
            .MigrateAsync(cancellationToken);
    }

    /// <summary>
    /// Creates users and roles.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    internal virtual async Task EnsureIdentityAsync(CancellationToken cancellationToken = default)
    {
        if (!ConfigManager.HasDbContext)
            return;

        if (this.Options.ConnectionString == null)
            return;

        var securityOptions = this.GetService<SecurityOptions>();

        await this.AddRole(BuiltInUserRoles.GUEST);
        await this.AddRole(BuiltInUserRoles.READER);
        await this.AddRole(BuiltInUserRoles.WRITER);
        await this.AddRole(BuiltInUserRoles.SERVICE);
        await this.AddRole(BuiltInUserRoles.ADMINISTRATOR);

        var adminPassword = securityOptions.User.AdminPassword;
        var adminEmailAddress = securityOptions.User.AdminEmailAddress;

        if (!string.IsNullOrEmpty(adminEmailAddress) && !string.IsNullOrEmpty(adminPassword))
        {
            var adminUser = await this.AddUser(adminEmailAddress, adminPassword);

            await this.AddUserToRole(adminUser, BuiltInUserRoles.SERVICE);
            await this.AddUserToRole(adminUser, BuiltInUserRoles.ADMINISTRATOR);
        }

        await this.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
            throw new ArgumentNullException(nameof(modelBuilder));

        base.OnModelCreating(modelBuilder);

        if (!string.IsNullOrEmpty(this.Options.DefaultCollation))
        {
            modelBuilder
                .UseCollation(this.Options.DefaultCollation);
        }

        modelBuilder
            .MapDefaultIdentity<TIdentity>()
            .AddMapping<DefaultAuditEntry, DefaultAuditEntryMapping>()
            .AddMapping<DefaultAuditEntryProperty, DefaultAuditEntryPropertyMapping>()
            .AddMapping<IdentityApiKey<TIdentity>, IdentityApiKeyMapping<TIdentity>>()
            .AddMapping<IdentityUserChangeData<TIdentity>, IdentityUserChangeDataMapping<TIdentity>>();
    }

    private void UpdateOriginalValues(object entity, object tracked = null, EntityEntry owner = null, string propertName = null)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

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

                    this.UpdateOriginalValues(value, valueTracked, entry, propertyInfo.Name);
                }
            }
        }
        finally
        {
            if (this.Options.UseLazyLoading)
            {
                this.ChangeTracker.LazyLoadingEnabled = true;
            }
        }
    }
    private void UpdateSoftDeletedEntities()
    {
        if (!this.Options.UseSoftDeletetion)
        {
            return;
        }

        this.ChangeTracker
            .Entries<IEntityDeletableSoft>()
            .Where(x => x.State == EntityState.Deleted)
            .ToList()
            .ForEach(x =>
            {
                x.State = EntityState.Modified;
                x.Entity.IsDeleted = DateTimeOffset.UtcNow.GetEpochTime();
            });
    }
    private void SetPendingEntityEvents()
    {
        if (!this.IsEntityEventEnabled)
        {
            return;
        }

        this.pendingEvents = this.ChangeTracker
            .Entries<IEntity>()
            .Where(x =>
                x.Entity.GetType().IsTypeOf(typeof(IEntityIdentity<>)) &&
                x.Entity.GetType().GetCustomAttributes<PublishAttribute>().Any() &&
                x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x =>
            {
                var type = this.GetPublishType(x);
                var entityEvent = this.GetPublishEntityEvent(x, type);
                var propertyNames = this.GetPublishProperties(x);

                switch (x.State)
                {
                    case EntityState.Deleted:
                        return entityEvent;

                    case EntityState.Modified:
                    {
                        var hasChanged = this.HasPublishModifiedPropertiesChanged(x, propertyNames);

                        if (!hasChanged)
                        {
                            return null;
                        }

                        break;
                    }
                }

                var entityEventData = this.GetEntityEventData(x, propertyNames);

                entityEvent.Data = entityEventData;

                return entityEvent;
            })
            .Where(x => x != null)
            .ToList();
    }
    private Type GetPublishType(EntityEntry entityEntry)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var type = entityEntry.Entity
            .GetType();

        var isAttributeDirectlyApplied = false;
        while (!isAttributeDirectlyApplied)
        {
            if (type == null)
            {
                break;
            }

            isAttributeDirectlyApplied = Attribute.IsDefined(type, typeof(PublishAttribute), false);

            if (!isAttributeDirectlyApplied)
            {
                type = type.BaseType;
            }
        }

        return type;
    }
    private EntityEvent GetPublishEntityEvent(EntityEntry entityEntry, Type type)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var state = entityEntry.State.ToString();
        var typeName = type.Name.Replace("Proxy", string.Empty);

        return entityEntry.Entity switch
        {
            IEntityIdentity<int> @int => new EntityEvent(@int.Id, typeName, state),
            IEntityIdentity<long> @long => new EntityEvent(@long.Id, typeName, state),
            IEntityIdentity<string> @string => new EntityEvent(@string.Id, typeName, state),
            IEntityIdentity<Guid> guid => new EntityEvent(guid.Id, typeName, state),
            IEntityIdentity<dynamic> dynamic => new EntityEvent(dynamic.Id, typeName, state),
            _ => null
        };
    }
    private string[] GetPublishProperties(EntityEntry entityEntry)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var type = entityEntry.Entity
            .GetType();

        var propertyNames = new List<string>();
        while (type is { IsAbstract: false } && type.IsTypeOf(typeof(IEntity)))
        {
            var attribute = (PublishAttribute)type
                .GetCustomAttributes(typeof(PublishAttribute))
                .FirstOrDefault();

            if (attribute == null)
            {
                type = type.BaseType;
                continue;
            }

            propertyNames
                .AddRange(attribute.PropertyNames);

            type = type.BaseType;
        }

        return propertyNames
            .Distinct()
            .ToArray();
    }
    private bool HasPublishModifiedPropertiesChanged(EntityEntry entityEntry, string[] publishProperties)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        foreach (var propertyName in publishProperties)
        {
            var propertyNameTemp = propertyName;
            var nestedEntityEntry = entityEntry;

            var indexOfDot = propertyNameTemp
                .IndexOf('.');

            while (indexOfDot > -1)
            {
                var name = propertyNameTemp[..indexOfDot];
                propertyNameTemp = propertyNameTemp[(indexOfDot + 1)..];

                nestedEntityEntry = nestedEntityEntry.References
                    .FirstOrDefault(x => x.Metadata.Name == name)?
                    .TargetEntry;

                if (nestedEntityEntry == null)
                {
                    throw new NullReferenceException(nameof(nestedEntityEntry));
                }

                indexOfDot = propertyNameTemp
                    .IndexOf('.');
            }

            var property = nestedEntityEntry.Entity
                .GetType()
                .GetProperty(propertyNameTemp);

            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var value = property
                .GetValue(nestedEntityEntry.Entity);

            if (property.PropertyType.IsSimple())
            {
                var orginalValue = nestedEntityEntry.OriginalValues
                    .GetValue<object>(propertyNameTemp);

                var hasChanged = !(value?.Equals(orginalValue) ?? orginalValue is null);

                if (hasChanged)
                {
                    return true;
                }
            }
            else
            {
                var properties = property.PropertyType
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    .Select(x => x.Name)
                    .ToArray();

                nestedEntityEntry = nestedEntityEntry.References
                    .FirstOrDefault(x => x.Metadata.Name == propertyNameTemp)?
                    .TargetEntry;

                var hasChanged = this.HasPublishModifiedPropertiesChanged(nestedEntityEntry, properties);

                if (hasChanged)
                {
                    return true;
                }
            }
        }

        return false;
    }
    private IDictionary<string, object> GetEntityEventData(EntityEntry entityEntry, string[] publishProperties)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var entityEventData = new Dictionary<string, object>();
        foreach (var propertyName in publishProperties)
        {
            string name;
            int indexOfDot;

            var value = entityEntry.Entity;
            var propertyNameTemp = propertyName;

            do
            {
                indexOfDot = propertyNameTemp
                    .IndexOf('.');

                name = indexOfDot > -1
                    ? propertyNameTemp[..indexOfDot]
                    : propertyNameTemp;

                propertyNameTemp = indexOfDot > -1
                    ? propertyNameTemp[(indexOfDot + 1)..]
                    : propertyNameTemp;

                var property = value?
                    .GetType()
                    .GetProperty(name);

                value = property?
                    .GetValue(value);
            } while (indexOfDot > -1);

            entityEventData
                .Add(name, value);
        }

        return entityEventData;
    }
    private async Task PublishEntityEvents()
    {
        try
        {
            var eventing = this.GetService<IEventing>();

            foreach (var @event in this.pendingEvents)
            {
                await eventing
                    .PublishAsync(@event, @event.Type);
            }
        }
        finally
        {
            this.pendingEvents = new List<EntityEvent>();
        }
    }
 
    private async Task AddRole(string role)
    {
        if (role == null)
            throw new ArgumentNullException(nameof(role));

        var roleManager = this.GetService<RoleManager<IdentityRole<TIdentity>>>();

        var exists = await roleManager
            .RoleExistsAsync(role);

        if (!exists)
        {
            var identityRole = new IdentityRole<TIdentity>(role);

            await roleManager
                .CreateAsync(identityRole);
        }
    }
    private async Task AddUserToRole(IdentityUser<TIdentity> user, string role)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (role == null)
            throw new ArgumentNullException(nameof(role));

        var userManager = this.GetService<UserManager<IdentityUser<TIdentity>>>();

        var isInRole = await userManager
            .IsInRoleAsync(user, role);

        if (!isInRole)
        {
            await userManager
                .AddToRoleAsync(user, role);
        }
    }
    private async Task<IdentityUser<TIdentity>> AddUser(string emailAddress, string password)
    {
        if (emailAddress == null)
            throw new ArgumentNullException(nameof(emailAddress));

        if (password == null)
            throw new ArgumentNullException(nameof(password));

        var userManager = this.GetService<UserManager<IdentityUser<TIdentity>>>();

        var user = await userManager
            .FindByEmailAsync(emailAddress);

        if (user == null)
        {
            user = new IdentityUser<TIdentity>
            {
                UserName = emailAddress,
                Email = emailAddress,
                EmailConfirmed = true,
                PhoneNumber = "+1-000-000-0000",
                PhoneNumberConfirmed = true
            };

            await userManager
                .CreateAsync(user, password);
        }
        else
        {
            var isValid = await userManager
                .CheckPasswordAsync(user, password);

            if (!isValid)
            {
                var token = await userManager
                    .GeneratePasswordResetTokenAsync(user);

                await userManager
                    .ResetPasswordAsync(user, token, password);
            }
        }

        return user;
    }
}
