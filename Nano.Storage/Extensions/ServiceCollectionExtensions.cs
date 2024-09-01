using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config.Extensions;
using Nano.Storage.Interfaces;

namespace Nano.Storage.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds storage provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddStorage<TProvider>(this IServiceCollection services)
        where TProvider : class, IStorageProvider
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = services
            .BuildServiceProvider()
            .GetService<StorageOptions>();

        services
            .AddSingleton<IStorageProvider, TProvider>()
            .AddSingleton(x => x
                .GetRequiredService<IStorageProvider>()
                .Configure(services, options));

        return services;
    }

    /// <summary>
    /// Add file storage to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfigOptions<StorageOptions>(configuration, StorageOptions.SectionName, out _);

        services
            .AddSingleton<IPathProvider, PathProvider>();

        return services;
    }
}