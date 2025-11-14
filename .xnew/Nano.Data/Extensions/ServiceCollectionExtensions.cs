using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Extensions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Identity.Extensions;
using Nano.Data.Interfaces;
using Nano.Data.Models;
using Nano.Repository;
using Nano.Security;
using Nano.Security.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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

        EntityFrameworkManager.IsCommunity = true;

        services
            .AddConfigSection<DataOptions>(DataOptions.SectionName, out var options);

        if (options == null)
        {
            throw new NullReferenceException(nameof(options));
        }

        services
            .AddDbContext<TProvider, TContext, TIdentity>(options)
            .AddIdentity<TContext, TIdentity>(options.Identity)
            .AddAudit(options)
            .AddCache(options);

        return services;
    }


    private static IServiceCollection AddDbContext<TProvider, TContext, TIdentity>(this IServiceCollection services, DataOptions options = null)
        where TProvider : class, IDataProvider
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
            .AddScoped<IRepository, DefaultRepository>();

        return services;
    }
    private static IServiceCollection AddIdentity<TContext, TIdentity>(this IServiceCollection services, IdentityOptions options = null)
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
            .AddIdentityStore<TContext, TIdentity>()
            .AddIdentityOptions(options);

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            });

        services
            .AddScoped<IIdentityRepository<TIdentity>, DefaultIdentityRepository<TIdentity>>();

        return services;
    }
    private static IServiceCollection AddIdentityStore<TContext, TIdentity>(this IServiceCollection services)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddIdentity<IdentityUser<TIdentity>, IdentityRole<TIdentity>>()
            .AddEntityFrameworkStores<TContext>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUser<TIdentity>>>(JwtBearerDefaults.AuthenticationScheme)
            .AddDefaultTokenProviders()
            .AddCustomTokenProvider();

        return services;
    }
    private static IServiceCollection AddIdentityOptions(this IServiceCollection services, IdentityOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(x =>
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
            });

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
                    .GetService<IHttpContextAccessor>(); // BUG: Check implementation of this, maybe we can do it more easily. Also the name is almost the same as one in NuGet


                var a = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
                var userId = Guid.TryParse(a.Value, out var result);

                //var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                //const string PREFIX = "Baerer ";
                //var value = authorizationHeader[PREFIX.Length..];

                var requestId = httpContextAccessor.HttpContext?.TraceIdentifier;
                var createdBy = httpContextAccessor.HttpContext?.GetJwtUserId()?.ToString(); // BUG: Only place HttpContextExtensions are needed

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
                .CacheAllQueriesExceptContainingTypes(cacheExpirationMode, TimeSpan.FromMinutes(options.Cache.ExpirationTimeoutInSeconds))
                .CacheAllQueriesExceptContainingTableNames(cacheExpirationMode, TimeSpan.FromMinutes(options.Cache.ExpirationTimeoutInSeconds), options.Cache.IgnoredTableNames)
                .UseMemoryCacheProvider()
                .UseCacheKeyPrefix(CACHE_KEY_PREFIX));

        services
            .AddMemoryCache(x =>
            {
                x.SizeLimit = options.Cache.MaxEntries;
                x.ExpirationScanFrequency = TimeSpan.FromMinutes(options.Cache.ExpirationScanFrequencyInSeconds);
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

        // BUG: 000: Move to data provider projects
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