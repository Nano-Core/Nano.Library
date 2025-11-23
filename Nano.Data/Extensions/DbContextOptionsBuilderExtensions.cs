using System;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.Extensions;

/// <summary>
/// Db Context Options Builder Extensions.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    internal static void AddDataContext(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));
        
        if (serviceProvider == null) 
            throw new ArgumentNullException(nameof(serviceProvider));

        var options = serviceProvider
            .GetRequiredService<DataOptions>();

        builder
            .EnableSensitiveDataLogging(options.UseSensitiveDataLogging)
            .ConfigureWarnings(x =>
            {
                x.Ignore(RelationalEventId.BoolWithDefaultWarning);
                x.Log(RelationalEventId.MultipleCollectionIncludeWarning);
                x.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
            })
            .UseLazyLoadingProxies(options.UseLazyLoading);

        if (options.Cache != null)
        {
            var secondLevelCacheInterceptor = serviceProvider
                .GetRequiredService<SecondLevelCacheInterceptor>();

            builder
                .AddInterceptors(secondLevelCacheInterceptor);
        }

        serviceProvider
            .GetRequiredService<IDataProvider>()
            .Configure(builder);
    }

}