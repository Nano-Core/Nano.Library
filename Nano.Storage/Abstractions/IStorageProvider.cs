using Microsoft.Extensions.DependencyInjection;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage.Abstractions;

/// <summary>
/// Storage Provider interface.
/// Defines the provider used for file storage in the application.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Configures the storage provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    /// <returns>The <see cref="IStorageProvider"/>.</returns>
    void Configure(IServiceCollection services, StorageOptions options);
}