using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing;
using Nano.Data.Identity.Authentication.Extensions;
using Nano.Data.Identity.Extensions;
using System;
using System.Linq;
using Z.EntityFramework.Extensions;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

/// <summary>
/// Provides extension methods for registering Nano Library data providers and related services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a data provider and <see cref="DbContext"/> to the service collection, using <see cref="Guid"/> as the default identity type.
    /// </summary>
    /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation to use.</typeparam>
    /// <typeparam name="TContext">The <see cref="DbContext"/> implementation to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddNanoData<TProvider, TContext>(this IServiceCollection services)
        where TProvider : IDataProvider
        where TContext : BaseDbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoData<TProvider, TContext, Guid>();

        services
            .AddScoped<BaseDbContext, TContext>();

        return services;
    }

    /// <summary>
    /// Adds a data provider and <see cref="DbContext"/> to the service collection, using a custom identity type.
    /// </summary>
    /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation to use.</typeparam>
    /// <typeparam name="TContext">The <see cref="DbContext"/> implementation to register.</typeparam>
    /// <typeparam name="TIdentity">The type of the identity key for entities.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the <see cref="DataOptions"/> configuration section cannot be loaded.</exception>
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
            .AddAudit<TIdentity>(options)
            .AddIdentity<TContext, TIdentity>(options.Identity);

        services
            .AddScoped<TContext>()
            .AddScoped<DbContext, TContext>()
            .AddScoped<BaseDbContext<TIdentity>, TContext>()
            .AddScoped<IRepository, Repository<TContext, TIdentity>>();

        services
            .AddScoped<IDbMigrationTask, DbMigrationTask<TIdentity>>();

        services
            .AddScoped<EntityEventingHandler<TIdentity>>()
            .AddScoped<IRegisterEntityEventingHandlersTask, RegisterEntityEventingHandlersTask>();

        services
            .AddAuthentication()
            .AddApiKeyAuthentication<TIdentity>(options.Identity?.ApiKey);

        return services;
    }


    private static IServiceCollection AddContext<TProvider, TContext>(this IServiceCollection services, DataOptions options)
        where TProvider : IDataProvider
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.ConnectionPool == null)
        {
            services
                .AddDbContext<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext<TProvider>(provider, options);
                });
        }
        else
        {
            services
                .AddDbContextPool<TContext>((provider, builder) =>
                {
                    builder
                        .AddDataContext<TProvider>(provider, options);
                }, options.ConnectionPool.PoolSize);
        }

        return services;
    }
    private static IServiceCollection AddAudit<TIdentity>(this IServiceCollection services, DataOptions options)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        AuditManager.DefaultConfiguration.Include<IEntityAuditable>();
        AuditManager.DefaultConfiguration.IncludeDataAnnotation();
        AuditManager.DefaultConfiguration.IncludeIdentity<TIdentity>(options.Identity);
        AuditManager.DefaultConfiguration.Exclude<AuditEntry<TIdentity>>();
        AuditManager.DefaultConfiguration.Exclude<AuditEntryProperty<TIdentity>>();
        AuditManager.DefaultConfiguration.Exclude<DataProtectionKey>();
        AuditManager.DefaultConfiguration.ExcludeDataAnnotation();

        AuditManager.DefaultConfiguration.UseUtcDateTime = true;
        AuditManager.DefaultConfiguration.AutoSavePreAction = AutoSavePreAction<TIdentity>;

        return services;
    }
 
    private static void AutoSavePreAction<TIdentity>(DbContext dbContext, Audit audit)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(audit);

        var httpContextAccessor = dbContext
            .GetService<IHttpContextAccessor>();

        var requestId = httpContextAccessor.HttpContext?.TraceIdentifier;
        var createdBy = httpContextAccessor.HttpContext == null 
            ? null 
            : httpContextAccessor.HttpContext.GetJwtUserId() ?? "Anonymous";

        var auditEntries = audit.Entries
            .Where(x => x.AuditEntryID == 0)
            .Select(x =>
            {
                var entityKey = x.GetEntityKey<TIdentity>();
                var entityState = x.GetEntityState();
                var entityTypeName = x.GetEntityTypeName();

                return new AuditEntry<TIdentity>
                {
                    CreatedBy = createdBy ?? x.CreatedBy,
                    EntityKey = entityKey,
                    EntityState = entityState,
                    EntitySetName = x.EntitySetName,
                    EntityTypeName = entityTypeName,
                    RequestId = requestId,
                    Properties = x.Properties
                        .Select(y => new AuditEntryProperty<TIdentity>
                        {
                            PropertyName = y.PropertyName,
                            RelationName = y.RelationName,
                            NewValue = y.NewValueFormatted,
                            OldValue = y.OldValueFormatted
                        })
                        .Where(y =>
                            y.NewValue != y.OldValue &&
                            y.PropertyName != x.Entry.GetKeyName())
                        .ToArray()
                };
            });

        dbContext
            .Set<AuditEntry<TIdentity>>()
            .AddRange(auditEntries);
    }
}