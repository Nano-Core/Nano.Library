using Microsoft.Extensions.DependencyInjection;
using Nano.Storage.Abstractions.Config;
using System;

namespace Nano.Storage.Abstractions;

/// <summary>
/// Defines a storage provider used to configure file storage services for the Nano application.
/// </summary>
/// <remarks>
///     Implementations are responsible for registering all required services (such as clients, health checks, and path providers)
///     into the dependency injection container based on the supplied <see cref="StorageOptions"/>.
/// </remarks>
public interface IStorageProvider
{
    /// <summary>
    /// Configures storage-related services for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the provider must register its services.</param>
    /// <param name="options">The non-null <see cref="StorageOptions"/> controlling how the provider is configured.</param>
    /// <remarks>
    ///     Implementations should treat this method as a one-time configuration step and must not assume it will be called more than once.
    ///     The method should only add services to <paramref name="services"/> and must not build or resolve a service provider.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
    void Configure(IServiceCollection services, StorageOptions options);
}