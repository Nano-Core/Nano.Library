using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Identity.Extensions;
using System;
using System.Linq;
using Nano.Common.Config.Extensions;
using Nano.Common.Identity.Extensions;
using Nano.Data.Repository;
using Z.EntityFramework.Extensions;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

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

        services
            .AddConfigSection<DataOptions>(DataOptions.SectionName, out var options);

        if (options == null)
        {
            return services;
        }

        EntityFrameworkManager.IsCommunity = true;

        services
            .AddSingleton(options.Identity);

        services
            .AddScoped<DbContextOptions, DbContextOptions<TContext>>()
            .AddScoped<DbContext, TContext>()
            .AddScoped<BaseDbContext<TIdentity>, TContext>()
            .AddSingleton<IDataProvider, TProvider>();

        if (options.UseConnectionPooling)
        {
            services
                .AddDbContextPool<TContext>((x, builder) =>
                {
                    builder
                        .AddDataContext(x);
                })
                .AddHealthChecks<TProvider>(options);
        }
        else
        {
            services
                .AddDbContext<TContext>((x, builder) =>
                {
                    builder
                        .AddDataContext(x);
                })
                .AddHealthChecks<TProvider>(options);
        }

        services
            .AddDataProtection()
            .PersistKeysToDbContext<TContext>();

        services
            .AddIdentity<TContext, TIdentity>(options.Identity)
            .AddAudit(options)
            .AddCache(options);

        services
            .AddScoped<IRepository, DefaultRepository>();

        services
            .AddHostedService<MigrateDatabaseStartupTask>();

        return services;
    }


    private static IServiceCollection AddAudit(this IServiceCollection services, DataOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (!options.UseAudit)
        {
            return services;
        }

        if (options.UseAudit)
        {
            AuditManager.DefaultConfiguration.UseUtcDateTime = true;
            AuditManager.DefaultConfiguration.Include<IEntityAuditable>();
            AuditManager.DefaultConfiguration.IncludeProperty<IEntityAuditable>();
            AuditManager.DefaultConfiguration.IncludeDataAnnotation();
            AuditManager.DefaultConfiguration.Exclude<IEntityAuditableNegated>();
            AuditManager.DefaultConfiguration.ExcludeDataAnnotation();
            AuditManager.DefaultConfiguration.AutoSavePreAction = (dbContext, audit) =>
            {
                var httpContextAccessor = dbContext
                    .GetService<IHttpContextAccessor>();

                var requestId = httpContextAccessor?.HttpContext?.TraceIdentifier;

                var createdBy = httpContextAccessor?.HttpContext?
                    .GetJwtUserId()?
                    .ToString();

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
    private static IServiceCollection AddCache(this IServiceCollection services, DataOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Cache == null)
        {
            return services;
        }

        const string CACHE_KEY_PREFIX = "EF_";

        var cacheExpirationMode = options.Cache.ExpirationMode
            .GetCacheExpirationMode();

        services
            .AddEFSecondLevelCache(x => x
                .SkipCachingCommands(y => y.ToLower().Contains("__ef"))
                .CacheAllQueriesExceptContainingTypes(cacheExpirationMode, options.Cache.ExpirationTimeout)
                .CacheAllQueriesExceptContainingTableNames(cacheExpirationMode, options.Cache.ExpirationTimeout, options.Cache.IgnoredTableNames)
                .UseMemoryCacheProvider()
                .UseCacheKeyPrefix(CACHE_KEY_PREFIX));

        services
            .AddMemoryCache(x =>
            {
                x.SizeLimit = options.Cache.MaxEntries;
                x.ExpirationScanFrequency = options.Cache.ExpirationScanFrequency;
            });

        return services;
    }


    private static IServiceCollection AddHealthChecks<TProvider>(this IServiceCollection services, DataOptions options)
        where TProvider : class, IDataProvider
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (!options.UseHealthCheck)
            return services;

        // BUG: HEALTH-CHECK: Move to data provider projects
        //if (typeof(TProvider) == typeof(MySqlProvider))
        //{
        //    services
        //        .AddHealthChecks()
        //        .AddMySql(options.ConnectionString, failureStatus: options.UnhealthyStatus);
        //}
        //else if (typeof(TProvider) == typeof(SqlServerProvider))
        //{
        //    services
        //        .AddHealthChecks()
        //        .AddSqlServer(options.ConnectionString, failureStatus: options.UnhealthyStatus);
        //}
        //else if (typeof(TProvider) == typeof(SqliteProvider))
        //{
        //    services
        //        .AddHealthChecks()
        //        .AddSqlite(options.ConnectionString, failureStatus: options.UnhealthyStatus);
        //}

        return services;
    }
}