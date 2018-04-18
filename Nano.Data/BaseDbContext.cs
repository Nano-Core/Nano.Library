using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Nano.Data.Attributes;
using Nano.Data.Models;
using Nano.Data.Models.Mappings;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Models.Interfaces;
using Nano.Security;
using Newtonsoft.Json;

namespace Nano.Data
{
    /// <inheritdoc />
    public abstract class BaseDbContext : IdentityDbContext
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder
                .AddMapping<DefaultAuditEntry, DefaultAuditEntryMapping>();

            modelBuilder
                .Entity<IdentityUserLogin<string>>()
                .ToTable("__EFAuthUserLogin")
                .HasKey(x => new { x.UserId, x.ProviderKey });

            modelBuilder
                .Entity<IdentityUserRole<string>>()
                .ToTable("__EFAuthUserRole")
                .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder
                .Entity<IdentityUserToken<string>>()
                .ToTable("__EFAuthUserToken")
                .HasKey(x => new { x.UserId, x.Value });

            modelBuilder
                .Entity<IdentityUserClaim<string>>()
                .ToTable("__EFAuthUserClaim");

            modelBuilder
                .Entity<IdentityUser>()
                .ToTable("__EFAuthUser");

            modelBuilder
                .Entity<IdentityRoleClaim<string>>()
                .ToTable("__EFAuthRoleClaim");

            modelBuilder
                .Entity<IdentityRole>()
                .ToTable("__EFAuthRole");
        }

        /// <summary>
        /// Imports data for all models annotated with <see cref="DataImportAttribute"/>.
        /// </summary>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureSeedAsync(CancellationToken cancellationToken = default)
        {
            var securityOptions = this.GetService<SecurityOptions>() ?? new SecurityOptions();

            await this.AddRole("guest");
            await this.AddRole("reader");
            await this.AddRole("writer");
            await this.AddRole("service");
            await this.AddRole("administrator");

            var adminUser = await this.AddUser(securityOptions.User.AdminUsername, securityOptions.User.AdminPassword, securityOptions.User.AdminEmailAddress);

            await this.AddUserToRole(adminUser, "service");
            await this.AddUserToRole(adminUser, "administrator");

            await this.SaveChangesAsync(cancellationToken)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        var logger = this.GetService<ILogger>();
                        var message = x.Exception.Message;
                        var excption = x.Exception.Flatten();

                        logger.LogError(excption, message);
                    }
                }, cancellationToken);
        }

        /// <summary>
        /// Imports data for all models annotated with <see cref="DataImportAttribute"/>.
        /// </summary>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureImportAsync(CancellationToken cancellationToken = default)
        {
            await Task.Factory.StartNew(() =>
            {
                AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(y => y.GetTypes())
                    .Where(y => y.GetCustomAttributes<DataImportAttribute>().Any())
                    .ToList()
                    .ForEach(async y =>
                    {
                        var attribute = y.GetCustomAttribute<DataImportAttribute>();

                        await this
                            .AddRangeAsync(attribute.Uri, y, cancellationToken);
                    });
            }, cancellationToken)
            .ContinueWith(x =>
            {
                if (x.IsFaulted)
                {
                    var logger = this.GetService<ILogger>();
                    var message = x.Exception.Message;
                    var excption = x.Exception.Flatten();

                    logger.LogError(excption, message);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Create database.
        /// </summary>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            if (!this.Options.UseCreateDatabase)
                return;

            await this.Database
                .EnsureCreatedAsync(cancellationToken)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        var logger = this.GetService<ILogger>();
                        var message = x.Exception.Message;
                        var excption = x.Exception.Flatten();

                        logger.LogError(excption, message);
                    }
                }, cancellationToken);
        }

        /// <summary>
        /// Migrate database.
        /// </summary>
        /// <returns>The <see cref="Task"/> (void).</returns>
        public virtual async Task EnsureMigratedAsync(CancellationToken cancellationToken = default)
        {
            if (!this.Options.UseMigrateDatabase)
                return;

            await this.Database
                .MigrateAsync(cancellationToken)
                .ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        var logger = this.GetService<ILogger>();
                        var message = x.Exception.Message;
                        var excption = x.Exception.Flatten();

                        logger.LogError(excption, message);
                    }
                }, cancellationToken);
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
                        var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                        var entities = (IEnumerable<object>)JsonConvert.DeserializeObject(json, collectionType);

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

        private async Task<IdentityRole> AddRole(string role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var roleManager = this.GetService<RoleManager<IdentityRole>>();

            var exists = await roleManager.RoleExistsAsync(role);
            if (!exists)
            {
                var identityRole = new IdentityRole(role);
                await roleManager.CreateAsync(identityRole);

                return identityRole;
            }

            return await roleManager.FindByNameAsync(role);
        }
        private async Task<IdentityUser> AddUser(string username, string password, string emailAddress)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));

            var userManager = this.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = username,
                    Email = emailAddress,
                    EmailConfirmed = true,
                    PhoneNumber = "+1-000-000-0000",
                    PhoneNumberConfirmed = true
                };

                await userManager.CreateAsync(user, password);

                return user;
            }

            return await userManager.FindByNameAsync(username);
        }
        private async Task AddUserToRole(IdentityUser user, string role)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var userManager = this.GetService<UserManager<IdentityUser>>();

            var isInRole = await userManager.IsInRoleAsync(user, role);
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}