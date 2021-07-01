using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Nano.Config;
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

namespace Nano.Data
{
    /// <inheritdoc />
    public abstract class BaseDbContext<TIdentity> : IdentityDbContext<IdentityUser<TIdentity>, IdentityRole<TIdentity>, TIdentity, IdentityUserClaim<TIdentity>, IdentityUserRole<TIdentity>, IdentityUserLogin<TIdentity>, IdentityRoleClaim<TIdentity>, IdentityUserTokenExpiry<TIdentity>>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Options.
        /// </summary>
        public DataOptions Options { get; set; }

        /// <summary>
        /// Audit Entries.
        /// </summary>
        public virtual DbSet<DefaultAuditEntry> Audit { get; set; } 

        /// <summary>
        /// Audit Entry Properties.
        /// </summary>
        public virtual DbSet<DefaultAuditEntryProperty> AuditProperties { get; set; }

        /// <summary>
        /// Auto Save.
        /// </summary>
        public virtual bool AutoSave => this.Options.UseAutoSave;

        /// <inheritdoc />
        protected BaseDbContext(DbContextOptions contextOptions, DataOptions dataOptions)
            : base(contextOptions)
        {
            this.Options = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
        }
        
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder); 

            modelBuilder
                .AddMapping<DefaultAuditEntry, DefaultAuditEntryMapping>()
                .AddMapping<DefaultAuditEntryProperty, DefaultAuditEntryPropertyMapping>();

            modelBuilder
                .Entity<IdentityUserLogin<Guid>>()
                .ToTable("__EFAuthUserLogin");

            modelBuilder
                .Entity<IdentityUserRole<Guid>>()
                .ToTable("__EFAuthUserRole");

            modelBuilder
                .Entity<IdentityUserTokenExpiry<Guid>>()
                .ToTable("__EFAuthUserToken");

            modelBuilder
                .Entity<IdentityUserClaim<Guid>>()
                .ToTable("__EFAuthUserClaim");

            modelBuilder
                .Entity<IdentityUser<Guid>>()
                .ToTable("__EFAuthUser");

            modelBuilder
                .Entity<IdentityUser<Guid>>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder
                .Entity<IdentityUser<Guid>>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();

            modelBuilder
                .Entity<IdentityRoleClaim<Guid>>()
                .ToTable("__EFAuthRoleClaim");

            modelBuilder
                .Entity<IdentityRole<Guid>>()
                .ToTable("__EFAuthRole");
        }

        /// <summary>
        /// Create database.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
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
        public virtual async Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
        {
            if (!ConfigManager.HasDbContext)
                return;

            if (!this.Options.UseMigrateDatabase)
                return;

            if (this.Options.ConnectionString == null)
                return;

            await this.Database
                .MigrateAsync(cancellationToken);
        }
        
        /// <summary>
        /// Creates users and roles.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureIdentityAsync(CancellationToken cancellationToken = default)
        {
            if (!ConfigManager.HasDbContext)
                return;
            
            if (this.Options.ConnectionString == null)
                return;

            var securityOptions = this.GetService<SecurityOptions>() ?? new SecurityOptions();

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

        /// <inheritdoc />
        public override int SaveChanges()
        {
            return this.SaveChanges(true);
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var pendingEvents = this.GetPendingEntityEvents();

            this.SaveSoftDeletion();

            // TODO: Fix Audit
            //var audit = new Audit();
            
            //if (this.Options.UseAudit)
            //    audit.PreSaveChanges(this);

            var success = base
                .SaveChanges(acceptAllChangesOnSuccess);

            var eventing = this.GetService<IEventing>();

            if (eventing == null)
                return success;

            this.ChangeTracker.LazyLoadingEnabled = false;

            try
            {
                pendingEvents
                    .ForEach(x =>
                    {
                        eventing
                            .PublishAsync(x, x.Type)
                            .ConfigureAwait(false);
                    });
            }
            finally
            {
                this.ChangeTracker.LazyLoadingEnabled = true;
            }

            // TODO: Fix Audit
            //if (this.Options.UseAudit)
            //{
            //    audit.PostSaveChanges();
            //    audit.Configuration.AutoSavePreAction?.Invoke(this, audit);

            //    base.SaveChanges();
            //}

            return success;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await this.SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var pendingEvents = this.GetPendingEntityEvents();

            this.SaveSoftDeletion();

            // TODO: Fix Audit
            //var audit = new Audit();
            
            //if (this.Options.UseAudit)
            //    audit.PreSaveChanges(this);
    
            var success = await this
                .SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess, cancellationToken)
                .ContinueWith(async x =>
                {
                    var eventing = this.GetService<IEventing>();

                    if (eventing == null)
                        return await x;

                    this.ChangeTracker.LazyLoadingEnabled = false;

                        foreach (var @event in pendingEvents)
                        {
                            await eventing
                                .PublishAsync(@event, @event.Type);
                        }

                    this.ChangeTracker.LazyLoadingEnabled = true;

                    return await x;
                }, cancellationToken);

            //if (this.Options.UseAudit)
            //{
            //    audit.PostSaveChanges();
            //    audit.Configuration.AutoSavePreAction?.Invoke(this, audit);
                
            //    await base.SaveChangesAsync(cancellationToken);
            //}

            return await success;
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

        private void SaveSoftDeletion()
        {
            if (!this.Options.UseSoftDeletetion)
                return;

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
        private List<EntityEvent> GetPendingEntityEvents()
        {
            return this.ChangeTracker
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
    }
}