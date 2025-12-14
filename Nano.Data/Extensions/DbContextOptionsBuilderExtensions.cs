using System;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.Extensions;

internal static class DbContextOptionsBuilderExtensions
{
    internal static DbContextOptionsBuilder AddDataContext(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider, DataOptions options)
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));

        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

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
            var interceptor = serviceProvider
                .GetRequiredService<SecondLevelCacheInterceptor>();

            builder
                .AddInterceptors(interceptor);
        }

        return builder;
    }
}