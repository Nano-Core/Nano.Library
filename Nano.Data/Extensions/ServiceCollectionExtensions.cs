using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing;
using Nano.Data.Identity.Authentication.Extensions;
using Nano.Data.Identity.Extensions;
using Nano.Eventing.Abstractions;
using System;
using System.Linq;
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
    /// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data</remarks>
    public static IServiceCollection AddNanoData<TProvider, TContext>(this IServiceCollection services)
        where TProvider : IDataProvider
        where TContext : DefaultDbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoData<TProvider, TContext, Guid>();

        services
            .AddScoped<DefaultDbContext, TContext>();

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
    /// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data</remarks>
    public static IServiceCollection AddNanoData<TProvider, TContext, TIdentity>(this IServiceCollection services)
        where TProvider : IDataProvider
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoConfigSection<DataOptions>(DataOptions.SectionName, out var options);

        if (options is null)
        {
            throw new InvalidOperationException($"Configuration section '{DataOptions.SectionName}' could not be loaded.");
        }

        EntityFrameworkManager.IsCommunity = true;

        TProvider.Configure(services, options);

        services
            .AddContext<TProvider, TContext>(options)
            .AddAudit(options.UseAudit)
            .AddCache(options.Cache)
            .AddIdentity<TContext, TIdentity>(options.Identity);

        services
            .AddScoped<TContext>()
            .AddScoped<DbContext, TContext>()
            .AddScoped<BaseDbContext<TIdentity>, TContext>()
            .AddScoped<IRepository, DefaultRepository<TContext, TIdentity>>();

        services
            .AddScoped<IDbMigrationTask, DbMigrationTask<TIdentity>>();

        services
            .AddScoped<IEventingHandler<EntityEvent>, EntityEventHandler<TIdentity>>()
            .AddScoped<IRegisterEntityEventHandlersTask, RegisterEntityEventHandlersTask>();

        services
            .AddAuthentication()
            .AddApiKeyAuthentication<TIdentity>(options.Identity?.Authentication.ApiKey);

        return services;
    }


    private static IServiceCollection AddContext<TProvider, TContext>(this IServiceCollection services, DataOptions options)
        where TProvider : IDataProvider
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.UseConnectionPooling)
        {
            services
                .AddDbContextPool<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext(provider, options);

                    TProvider.Configure(builder, options);
                });
        }
        else
        {
            services
                .AddDbContext<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext(provider, options);

                    TProvider.Configure(builder, options);
                });
        }

        return services;
    }
    private static IServiceCollection AddAudit(this IServiceCollection services, bool useAudit = false)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (!useAudit)
        {
            AuditManager.DefaultConfiguration.Exclude(_ => true);
            AuditManager.DefaultConfiguration.AutoSavePreAction = null;

            return services;
        }

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

            var requestId = httpContextAccessor.HttpContext?.TraceIdentifier;

            var createdBy = httpContextAccessor.HttpContext?
                .GetJwtUserId()?
                .ToString();

            var auditEntries = audit.Entries
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
                .AddRange(auditEntries);
        };
        AuditManager.DefaultConfiguration.SoftDeleted<IEntityDeletableSoft>(x => x.IsDeleted > 0L);

        return services;
    }
    private static IServiceCollection AddCache(this IServiceCollection services, CacheOptions? options)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        const string CACHE_KEY_PREFIX = "EF_";

        var cacheExpirationMode = options.ExpirationMode
            .GetCacheExpirationMode();

        services
            .AddEFSecondLevelCache(x => x
                .SkipCachingCommands(y => y.ToLower().Contains("__ef", StringComparison.Ordinal))
                .CacheAllQueriesExceptContainingTypes(cacheExpirationMode, options.ExpirationTimeout)
                .CacheAllQueriesExceptContainingTableNames(cacheExpirationMode, options.ExpirationTimeout, options.IgnoredTableNames)
                .UseMemoryCacheProvider()
                .UseCacheKeyPrefix(CACHE_KEY_PREFIX));

        services
            .AddMemoryCache(x =>
            {
                x.SizeLimit = options.MaxEntries;
                x.ExpirationScanFrequency = options.ExpirationScanFrequency;
            });

        return services;
    }
}