using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config.Extensions;
using Nano.Data.Interfaces;
using Nano.Data.Providers.MySql;
using Nano.Data.Providers.SqlServer;
using Nano.Models.Interfaces;
using Nano.Security.Extensions;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nano.Config;
using Nano.Data.Models;
using Nano.Data.Providers.Sqlite;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds data provider for <see cref="DbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation.</typeparam>
        /// <typeparam name="TContext">The <see cref="DbContext"/> implementation.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TProvider, TContext>(this IServiceCollection services)
            where TProvider : class, IDataProvider
            where TContext : DefaultDbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<DefaultDbContext, TContext>();

            services
                .AddDataContext<TProvider, TContext, Guid>();

            return services;
        }

        /// <summary>
        /// Adds data provider for <see cref="DbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation.</typeparam>
        /// <typeparam name="TContext">The <see cref="DbContext"/> implementation.</typeparam>
        /// <typeparam name="TIdentity">The identity type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TProvider, TContext, TIdentity>(this IServiceCollection services)
            where TProvider : class, IDataProvider
            where TContext : BaseDbContext<TIdentity>
            where TIdentity : IEquatable<TIdentity>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = services.BuildServiceProvider()
                .GetRequiredService<DataOptions>();

            services
                .AddScoped<DbContext, TContext>()
                .AddScoped<BaseDbContext<TIdentity>, TContext>()
                .AddScoped<DbContextOptions, DbContextOptions<TContext>>()
                .AddSingleton<IDataProvider, TProvider>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder);
                })
                .AddDataHealthChecks<TProvider>(options);

            services
                .AddIdentity<IdentityUser<TIdentity>, IdentityRole<TIdentity>>()
                .AddEntityFrameworkStores<BaseDbContext<TIdentity>>()
                .AddTokenProvider<DataProtectorTokenProvider<IdentityUser<TIdentity>>>(JwtBearerDefaults.AuthenticationScheme)
                .AddDefaultTokenProviders();

            services
                .AddIdentityManager<TIdentity>();

            ConfigManager.HasDbContext = true;

            return services;
        }

        /// <summary>
        /// Adds <see cref="DataOptions"/> appOptions to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddConfigOptions<DataOptions>(configuration, DataOptions.SectionName, out var options);

            services
                .AddScoped<DbContext, NullDbContext>()
                .AddScoped<BaseDbContext<Guid>, NullDbContext>()
                .AddScoped<DefaultDbContext, NullDbContext>()
                .AddScoped<DbContextOptions<NullDbContext>>()
                .AddScoped<DbContextOptions, DbContextOptions<NullDbContext>>();

            services
                .AddIdentityManager<Guid>();

            services
                .AddAudit(options)
                .AddDataCache(options);

            return services;
        }

        private static IServiceCollection AddAudit(this IServiceCollection services, DataOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.UseAudit)
            {
                AuditManager.DefaultConfiguration.Include<IEntityAuditable>();
                AuditManager.DefaultConfiguration.IncludeProperty<IEntityAuditable>();
                AuditManager.DefaultConfiguration.IncludeDataAnnotation();
                AuditManager.DefaultConfiguration.Exclude<IEntityAuditableNegated>();
                AuditManager.DefaultConfiguration.ExcludeDataAnnotation();
                AuditManager.DefaultConfiguration.AutoSavePreAction = (dbContext, audit) =>
                {
                    var httpContextAccessor = services
                        .BuildServiceProvider()
                        .GetService<IHttpContextAccessor>();

                    var requestId = httpContextAccessor?.HttpContext?.TraceIdentifier;
                    var createdBy = httpContextAccessor?.HttpContext?.GetJwtUserId()?.ToString();

                    var customAuditEntries = audit.Entries
                        .Where(x => x.AuditEntryID == 0)
                        .Select(x =>
                        {
                            return new DefaultAuditEntry
                            {
                                CreatedBy = createdBy ?? x.CreatedBy,
                                EntitySetName = x.EntitySetName,
                                EntityTypeName = x.EntityTypeName,
                                State = (int)x.State,
                                StateName = x.StateName,
                                RequestId = requestId,
                                Properties = x.Properties
                                    .Select(y => new DefaultAuditEntryProperty
                                    {
                                        PropertyName = y.PropertyName,
                                        RelationName = y.RelationName,
                                        NewValue = y.NewValueFormatted,
                                        OldValue = y.OldValueFormatted
                                    })
                                    .ToArray()
                            };
                        });

                    dbContext
                        .Set<DefaultAuditEntry>()
                        .AddRange(customAuditEntries);
                };
                AuditManager.DefaultConfiguration.SoftDeleted<IEntityDeletableSoft>(x => x.IsDeleted > 0L);
            }
            else
            {
                AuditManager.DefaultConfiguration.Exclude(_ => true);
                AuditManager.DefaultConfiguration.AutoSavePreAction = null;
            }

            return services;
        }
        private static IServiceCollection AddDataCache(this IServiceCollection services, DataOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (!options.UseMemoryCache)
                return services;

            services
                .AddMemoryCache(x =>
                {
                    x.CompactionPercentage = 25;
                    x.ExpirationScanFrequency = TimeSpan.FromMinutes(15);
                });

            return services;
        }
        private static IServiceCollection AddDataHealthChecks<TProvider>(this IServiceCollection services, DataOptions options)
            where TProvider : class, IDataProvider
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (!options.UseHealthCheck)
                return services;

            if (typeof(TProvider) == typeof(MySqlProvider))
            {
                services
                    .AddHealthChecks()
                        .AddMySql(options.ConnectionString);
            }
            else if (typeof(TProvider) == typeof(SqlServerProvider))
            {
                services
                    .AddHealthChecks()
                        .AddSqlServer(options.ConnectionString);
            }
            else if (typeof(TProvider) == typeof(SqliteProvider))
            {
                services
                    .AddHealthChecks()
                        .AddSqlite(options.ConnectionString);
            }

            return services;
        }
    }
}