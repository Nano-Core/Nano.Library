using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Eventing.Interceptors;
using Nano.Data.Interceptors;
using Nano.Eventing.Abstractions;
using System;

namespace Nano.Data.Extensions;

internal static class DbContextOptionsBuilderExtensions
{
    internal static DbContextOptionsBuilder AddDataContext<TProvider>(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider, DataOptions options)
        where TProvider : IDataProvider
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);

        builder
            .ConfigureLoggingAndWarnings(options)
            .ConfigureLazyLoading(options)
            .ConfigureEntityEventing(serviceProvider)
            .ConfigureSoftDelete();

        TProvider.Configure(builder, options);

        return builder;
    }


    private static DbContextOptionsBuilder ConfigureLoggingAndWarnings(this DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder
            .EnableSensitiveDataLogging(options.UseSensitiveDataLogging)
            .ConfigureWarnings(x =>
            {
                x.Ignore(RelationalEventId.BoolWithDefaultWarning);
                x.Log(RelationalEventId.MultipleCollectionIncludeWarning);
                x.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
            });

        return builder;
    }
    private static DbContextOptionsBuilder ConfigureLazyLoading(this DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.UseLazyLoading)
        {
            return builder;
        }

        builder
            .UseLazyLoadingProxies(options.UseLazyLoading);

        return builder;
    }
    private static DbContextOptionsBuilder ConfigureEntityEventing(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var eventing = serviceProvider
            .GetService<IEventing>();

        if (eventing != null)
        {
            builder
                .AddInterceptors(new EntityEventingSaveChangesInterceptor(eventing));
        }


        return builder;
    }
    private static DbContextOptionsBuilder ConfigureSoftDelete(this DbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .AddInterceptors(new SoftDeleteSaveChangesInterceptor());

        return builder;
    }
}