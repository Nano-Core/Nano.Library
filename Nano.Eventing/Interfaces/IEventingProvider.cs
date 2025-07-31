using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Eventing.Interfaces;

namespace Nano.Eventing.Interfaces;

/// <summary>
/// Eventing Provider interface.
/// Defines the provider used for eventing in the application.
/// </summary>
public interface IEventingProvider
{
    /// <summary>
    /// Configures the <see cref="IEventing"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="EventingOptions"/>.</param>
    void Configure(IServiceCollection services, EventingOptions options);
}