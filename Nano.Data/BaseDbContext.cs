using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.Config;
using Nano.Data.Const;
using Nano.Data.Models;
using Nano.Data.Models.Mappings;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Eventing;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Security;
using Nano.Security.Const;
using Nano.Security.Data.Models;
using Serilog;

namespace Nano.Data;

/// <inheritdoc />
public abstract class BaseDbContext<TIdentity> : IdentityDbContext<IdentityUser<TIdentity>, IdentityRole<TIdentity>, TIdentity, IdentityUserClaim<TIdentity>, IdentityUserRole<TIdentity>, IdentityUserLogin<TIdentity>, IdentityRoleClaim<TIdentity>, IdentityUserTokenExpiry<TIdentity>>
    where TIdentity : IEquatable<TIdentity>
{
    private IEnumerable<EntityEvent> pendingEvents;

    /// <summary>
    /// Options.
    /// </summary>
    public DataOptions Options { get; }

    /// <summary>
    /// Auto Save.
    /// </summary>
    public virtual bool AutoSave => this.Options.UseAutoSave;

    /// <inheritdoc />
    protected BaseDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
        : base(contextOptions)
    {
        this.Options = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));

        this.SavingChanges += (_, _) => this.SavePendingEntityEvents();
        this.SavingChanges += (_, _) => this.SaveSoftDeletion();
        this.SavedChanges += async (_, _) => await this.ExecuteEntityEvents();
    }

    /// <inheritdoc />
    public override EntityEntry Update(object entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (this.Options.UseAudit)
        {
            // TODO: Audit is not aware of existing values when updating. We need to pull the existing entity and compare properties.

            //var type = entity
            //    .GetType();

            //var key = typeof(TIdentity)
            //    .GetProperty(nameof(IEntityIdentity<TIdentity>.Id));

            //var keyValue = key?
            //    .GetValue(entity);

            //var existingEntity = this.Find(type, keyValue);

            //if (existingEntity == null)
            //{
            //    return base.Update(entity);
            //}

            //var properties = type
            //    .GetProperties();

            //foreach (var property in properties)
            //{
            //    var value = property
            //        .GetValue(entity);

            //    if (property.CanWrite)
            //    {
            //        property
            //            .SetValue(existingEntity, value);
            //    }
            //}

            //return base.Update(existingEntity);
        }

        return base.Update(entity);
    }

    /// <inheritdoc />
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (this.Options.UseAudit)
        {
            // TODO: Audit is not aware of existing values when updating. We need to pull the existing entity and compare properties.

            //var key = typeof(TEntity)
            //    .GetProperty(nameof(IEntityIdentity<TIdentity>.Id));

            //var keyValue = key?
            //    .GetValue(entity);

            //var existingEntity = this.Find<TEntity>(keyValue);

            //if (existingEntity == null)
            //{
            //    return base.Update(entity);
            //}

            //var properties = typeof(TEntity)
            //    .GetProperties();//.Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string));

            //foreach (var property in properties)
            //{
            //    var value = property
            //        .GetValue(entity);

            //    if (property.CanWrite)
            //    {
            //        property
            //            .SetValue(existingEntity, value);
            //    }
            //}

            //return base.Update(existingEntity);
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
            .AddMapping<DefaultAuditEntry, DefaultAuditEntryMapping>()
            .AddMapping<DefaultAuditEntryProperty, DefaultAuditEntryPropertyMapping>();

        modelBuilder
            .Entity<IdentityUserLogin<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_LOGIN_TABLE_NAME);

        modelBuilder
            .Entity<IdentityUserRole<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_ROLE);

        modelBuilder
            .Entity<IdentityUserTokenExpiry<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_TOKEN_EXPIRY);

        modelBuilder
            .Entity<IdentityUserClaim<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_CLAIM);

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER);

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();

        modelBuilder
            .Entity<IdentityRoleClaim<TIdentity>>()
            .ToTable(TableNames.IDENTITY_ROLE_CLAIM);

        modelBuilder
            .Entity<IdentityRole<TIdentity>>()
            .ToTable(TableNames.IDENTITY_ROLE);
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
    internal virtual async Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
    {
        if (!ConfigManager.HasDbContext)
            return;

        if (!this.Options.UseMigrateDatabase)
            return;

        if (this.Options.ConnectionString == null)
            return;

        Log.Information("Applying Migrations at start-up.");

        await this.Database
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

        if (!securityOptions.IsAuth)
            return;

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

        await base.SaveChangesAsync(cancellationToken);
    }

    private void SaveSoftDeletion()
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
    private void SavePendingEntityEvents()
    {
        this.pendingEvents = this.ChangeTracker
            .Entries<IEntity>()
            .Where(x =>
                x.Entity.GetType().IsTypeOf(typeof(IEntityIdentity<>)) &&
                x.Entity.GetType().GetCustomAttributes<PublishAttribute>().Any() &&
                x.State is EntityState.Added or EntityState.Deleted)
            .Select(x =>
            {
                var type = x.Entity.GetType();
                var state = x.State.ToString();
                var name = type.Name.Replace("Proxy", string.Empty);

                return x.Entity switch
                {
                    IEntityIdentity<int> @int => new EntityEvent(@int.Id, name, state),
                    IEntityIdentity<long> @long => new EntityEvent(@long.Id, name, state),
                    IEntityIdentity<string> @string => new EntityEvent(@string.Id, name, state),
                    IEntityIdentity<Guid> guid => new EntityEvent(guid.Id, name, state),
                    IEntityIdentity<dynamic> dynamic => new EntityEvent(dynamic.Id, name, state),
                    _ => null
                };
            })
            .Where(x => x != null)
            .ToList();
    }
    private async Task ExecuteEntityEvents()
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
            this.pendingEvents = null;
        }
    }
    private async Task<IdentityRole<TIdentity>> AddRole(string role)
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

            return identityRole;
        }

        return await roleManager
            .FindByNameAsync(role);
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
}
