using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Eventing.Interfaces;

namespace Nano.Storage.Interfaces;

/// <summary>
/// Storage Provider interface.
/// Defines the provider used for storage in the application.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Configures the storage provider.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    /// <returns>The <see cref="IEventing"/>.</returns>
    IStorageProvider Configure(IServiceCollection services, StorageOptions options);
}