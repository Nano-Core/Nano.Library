using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Common.Identity.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Identity;
using Nano.Data.Identity.Consts;
using Nano.Data.Identity.DataProtection.Extensions;
using System;
using System.Linq;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Eventing;
using Nano.Eventing.Abstractions;
using Z.EntityFramework.Extensions;
using Z.EntityFramework.Plus;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

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
        where TProvider : class, IDataProvider, new()
        where TContext : DefaultDbContext
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddDataContext<TProvider, TContext, Guid>();

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
    public static IServiceCollection AddDataContext<TProvider, TContext, TIdentity>(this IServiceCollection services)
        where TProvider : class, IDataProvider, new()
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

        var provider = new TProvider();
        provider
            .Configure(services, options);

        services
            .AddSingleton<IDataProvider>(provider)
            .AddContext<TContext>(options)
            .AddIdentity<TContext, TIdentity>(options.Identity)
            .AddAudit(options)
            .AddCache(options.Cache);

        services
            .AddScoped<TContext>()
            .AddScoped<DbContext, TContext>()
            .AddScoped<BaseDbContext<TIdentity>, TContext>()
            .AddScoped<IRepository, DefaultRepository<TContext, TIdentity>>();

        services
            .AddScoped<IEventingHandler<EntityEvent>, EntityEventHandler<TIdentity>>();

        services
            .AddSingleton<IDbMigrationTask, DbMigrationTask<TIdentity>>()
            .AddSingleton<IRegisterEntityEventHandlersTask, RegisterEntityEventHandlersTask>();

        return services;
    }


    private static IServiceCollection AddContext<TContext>(this IServiceCollection services, DataOptions options)
        where TContext : DbContext
    {
        if (options.UseConnectionPooling)
        {
            services
                .AddDbContextPool<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext(provider, options);

                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder, options);
                });
        }
        else
        {
            services
                .AddDbContext<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext(provider, options);

                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder, options);
                });
        }

        return services;
    }
    private static IServiceCollection AddIdentity<TContext, TIdentity>(this IServiceCollection services, IdentityOptions options)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddIdentity<IdentityUserExt<TIdentity>, IdentityRole<TIdentity>>(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters;

                x.Password.RequireDigit = options.Password.RequireDigit;
                x.Password.RequiredLength = options.Password.RequiredLength;
                x.Password.RequireNonAlphanumeric = options.Password.RequireNonAlphanumeric;
                x.Password.RequireLowercase = options.Password.RequireLowercase;
                x.Password.RequireUppercase = options.Password.RequireUppercase;
                x.Password.RequiredUniqueChars = options.Password.RequiredUniqueCharacters;

                x.SignIn.RequireConfirmedEmail = options.SignIn.RequireConfirmedEmail;
                x.SignIn.RequireConfirmedPhoneNumber = options.SignIn.RequireConfirmedPhoneNumber;

                x.Lockout.AllowedForNewUsers = options.Lockout.AllowedForNewUsers;
                x.Lockout.DefaultLockoutTimeSpan = options.Lockout.DefaultLockoutTimeSpan;
                x.Lockout.MaxFailedAccessAttempts = options.Lockout.MaxFailedAccessAttempts;
            })
            .AddEntityFrameworkStores<TContext>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUserExt<TIdentity>>>(AuthenticationSchemes.JWT_BEARER)
            .AddDefaultTokenProviders()
            .AddCustomTokenProvider();

        services
            .AddDataProtection()
            .PersistKeysToDbContext<TContext>();

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            });

        services
            .AddScoped<IIdentityRepository, DefaultIdentityRepository>()
            .AddScoped<IIdentityRepository<TIdentity>, DefaultIdentityRepository<TIdentity>>(); 

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
        }
        else
        {
            AuditManager.DefaultConfiguration.Exclude(_ => true);
            AuditManager.DefaultConfiguration.AutoSavePreAction = null;
        }

        return services;
    }
    private static IServiceCollection AddCache(this IServiceCollection services, CacheOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        const string CACHE_KEY_PREFIX = "EF_";

        var cacheExpirationMode = options.ExpirationMode
            .GetCacheExpirationMode();

        services
            .AddEFSecondLevelCache(x => x
                .SkipCachingCommands(y => y.ToLower().Contains("__ef"))
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