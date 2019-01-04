using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config.Extensions;
using Nano.Data.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Security.Extensions;
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
                .AddScoped<DbContext, TContext>()
                .AddScoped<BaseDbContext, TContext>()
                .AddScoped<DefaultDbContext, TContext>()
                .AddSingleton<IDataProvider, TProvider>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder);
                });

            var dbContext = services
                .BuildServiceProvider()
                .GetService<TContext>();

            dbContext?.EnsureCreatedAsync().Wait();
            dbContext?.EnsureMigratedAsync().Wait();
            dbContext?.EnsureSeedAsync().Wait();
            dbContext?.EnsureImportAsync().Wait();

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
                .AddScoped<DbContext, DefaultDbContext>()
                .AddScoped<BaseDbContext, DefaultDbContext>()
                .AddScoped<DbContextOptions, DbContextOptions<DefaultDbContext>>();

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<BaseDbContext>()
                .AddDefaultTokenProviders();

            services.AddAudit(options);
            services.AddDataCache(options);

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
                    var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
                    var httpContext = httpContextAccessor?.HttpContext;

                    var createdBy = httpContext?.Request.GetUser();
                    var requestId = httpContext?.TraceIdentifier;

                    var customAuditEntries = audit.Entries
                        .Select(x =>
                        {
                            var id = Guid.NewGuid();

                            return new DefaultAuditEntry
                            {
                                Id = id,
                                CreatedBy = createdBy ?? x.CreatedBy,
                                EntitySetName = x.EntitySetName,
                                EntityTypeName = x.EntityTypeName,
                                State = (int) x.State,
                                StateName = x.StateName,
                                RequestId = requestId,
                                Properties = x.Properties
                                    .Select(y => new DefaultAuditEntryProperty
                                    {
                                        ParentId = id,
                                        NewValue = y.NewValueFormatted,
                                        OldValue = y.OldValueFormatted,
                                        PropertyName = y.PropertyName,
                                        RelationName = y.RelationName
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
                AuditManager.DefaultConfiguration.Exclude(x => true);
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

            // TODO: Data cache (custom / distributed, e.g. redis) (https://github.com/VahidN/EFSecondLevelCache.Core/)

            services
                .AddMemoryCache(cacheOptions => cacheOptions.ExpirationScanFrequency = TimeSpan.FromMinutes(15));            

            return services;
        }
    }
}