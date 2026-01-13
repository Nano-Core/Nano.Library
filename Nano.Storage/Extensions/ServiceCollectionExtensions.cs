using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using System;

namespace Nano.Storage.Extensions;

/// <summary>
/// Provides extension methods for registering Nano storage services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a storage provider and related services.
    /// </summary>
    /// <typeparam name="TProvider">The <see cref="IStorageProvider"/> implementation used to configure storage services.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which storage services are added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance to allow fluent configuration.</returns>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Loads <see cref="StorageOptions"/> from configuration.</description>
    ///   </item>
    ///   <item>
    ///     <description>Instantiates <typeparamref name="TProvider"/> and invokes <see cref="IStorageProvider.Configure"/>.</description>
    ///   </item>
    ///   <item>
    ///     <description>Registers <see cref="IStorageProvider"/> and <see cref="IPathProvider"/> in the dependency injection container.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddNanoStorage<TProvider>(this IServiceCollection services)
        where TProvider : class, IStorageProvider, new()
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoConfigSection<StorageOptions>(StorageOptions.SectionName, out var options);

        if (options is null)
        {
            throw new InvalidOperationException($"Configuration section '{StorageOptions.SectionName}' could not be loaded.");
        }

        var provider = Activator.CreateInstance<TProvider>();
        provider
            .Configure(services, options);

        services
            .AddSingleton<IStorageProvider>(provider)
            .AddSingleton<IPathProvider, PathProvider>();

        return services;
    }
}