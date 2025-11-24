using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions.Config;

namespace Nano.Eventing.Abstractions;

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
    void Configure(IServiceCollection services);
}