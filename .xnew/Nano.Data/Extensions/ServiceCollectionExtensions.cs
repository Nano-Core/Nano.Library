using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Data.Identity.Extensions;
using Nano.Data.Interfaces;
using Nano.Data.Models;
using Nano.Models.Interfaces;
using Nano.Repository;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Nano.Web.Hosting.Authentication;
using Nano.Web.Hosting.Authentication.Const;
using Z.EntityFramework.Extensions;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

// BUG: ALL: Look through all option classes and remove " = new() "

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

        services
            .AddDbContext<TProvider, TContext, TIdentity>()
            .AddIdentity<TContext, TIdentity>(options.Identity)
            .AddAudit(options)
            .AddCache(options);

        services
            .AddScoped<IRepository, DefaultRepository>();

        ConfigManager.HasDbContext = true;

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
            .AddIdentityJwtPublicKey()
            .AddIdentityOptions();

        services
            .AddSecurity(options);

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            });

        services
            .AddScoped<IIdentityManager<TIdentity>, DefaultIdentityRepository<TIdentity>>();

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
    private static IServiceCollection AddIdentityJwtPublicKey(this IServiceCollection services, JwtAuthenticationOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddSingleton(_ =>
            {
                var rsaSecurityKey = RSA.Create();
                var publicKey = Convert.FromBase64String(options.PublicKey);

                rsaSecurityKey
                    .ImportRSAPublicKey(publicKey, out var _);

                return new RsaSecurityKey(rsaSecurityKey);
            });

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
    private static IServiceCollection AddAudit(this IServiceCollection services, DataOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
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
    private static IServiceCollection AddCache(this IServiceCollection services, DataOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options?.Cache == null)
        {
            return services;
        }

        if (!options.UseMemoryCache) // BUG: DATA: Needed? just null check
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



    private static IServiceCollection AddSecurity(this IServiceCollection services, IdentityOptions securityOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (securityOptions == null)
            throw new ArgumentNullException(nameof(securityOptions));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap
            .Clear();

        var authenticationSchemes = new List<string>();

        if (securityOptions.Authentication.Jwt.IsEnabled)
        {
            authenticationSchemes
                .Add(JwtBearerDefaults.AuthenticationScheme);
        }

        if (securityOptions.Authentication.ApiKey != null)
        {
            authenticationSchemes
                .Add(ApiKeyDefaults.AuthenticationScheme);
        }

        services
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                x.AddPolicy(AuthenticationPolicyDefaults.POLICY, y => y
                    .AddAuthenticationSchemes(authenticationSchemes.ToArray())
                    .RequireAuthenticatedUser());
            });

        var defaultAuthenticationScheme = securityOptions.Authentication.Jwt.IsEnabled
            ? JwtBearerDefaults.AuthenticationScheme
            : securityOptions.Authentication.ApiKey != null
                ? ApiKeyDefaults.AuthenticationScheme
                : null;

        var authenticationBuilder = services
            .AddAuthentication(x =>
            {
                x.DefaultScheme = defaultAuthenticationScheme;
                x.DefaultChallengeScheme = defaultAuthenticationScheme;
                x.DefaultAuthenticateScheme = defaultAuthenticationScheme;
                x.DefaultForbidScheme = defaultAuthenticationScheme;
                x.DefaultSignInScheme = defaultAuthenticationScheme;
                x.DefaultSignOutScheme = defaultAuthenticationScheme;
            });

        if (securityOptions.Authentication.Jwt.IsEnabled)
        {
            var rsaSecurityKey = services
                .BuildServiceProvider()
                .GetRequiredService<RsaSecurityKey>();

            authenticationBuilder
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.IncludeErrorDetails = true;
                    x.RequireHttpsMetadata = false;

                    x.Audience = securityOptions.Authentication.Jwt.Audience;
                    x.ClaimsIssuer = securityOptions.Authentication.Jwt.Issuer;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = securityOptions.Authentication.Jwt.Issuer,
                        ValidAudience = securityOptions.Authentication.Jwt.Audience,
                        IssuerSigningKey = rsaSecurityKey,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                const string KEY = "Token-Expired";

                                context.Response.Headers
                                    .TryAdd(KEY, true.ToString());
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddExternalLogins(securityOptions);
        }

        if (securityOptions.Authentication.ApiKey != null)
        {
            services
                .AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyDefaults.AuthenticationScheme, _ => { });
        }

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

        // BUG: DATA: Move to data provider projects
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