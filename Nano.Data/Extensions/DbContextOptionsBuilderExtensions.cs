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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);

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