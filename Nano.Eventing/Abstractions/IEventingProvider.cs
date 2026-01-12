using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions.Config;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// Defines a generic interface for an eventing provider in the Nano application.
/// Eventing providers are responsible for delivering and handling events, and this interface allows different '
/// implementations (e.g., RabbitMQ, Azure Service Bus, Kafka, etc.).
/// </summary>
public interface IEventingProvider
{
    /// <summary>
    /// Configures the eventing services for the Nano application.
    /// </summary>
    /// <param name="services">The service collection used to register dependencies.</param>
    /// <param name="options">The <see cref="EventingOptions"/> containing configuration settings for the eventing provider.</param>
    /// <remarks>
    ///     This method should register any necessary services, connections, or middleware
    ///     required by the specific eventing provider implementation.
    /// </remarks>
    void Configure(IServiceCollection services, EventingOptions options);
}