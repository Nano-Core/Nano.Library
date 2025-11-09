using System;
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

        services
            .AddConfigSection<StorageOptions>(StorageOptions.SectionName, out var options);

        if (options == null)
        {
            return services;
        }

        services
            .AddSingleton<IStorageProvider, TProvider>()
            .AddSingleton(x =>
            {
                var options = x
                    .GetRequiredService<StorageOptions>();

                return x
                    .GetRequiredService<IStorageProvider>()
                    .Configure(services, options);
            });

        services
            .AddSingleton<IPathProvider, PathProvider>();

        return services;
    }
}