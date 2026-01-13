using Microsoft.Extensions.DependencyInjection;
using Nano.Logging.Abstractions.Config;

namespace Nano.Logging.Abstractions;

/// <summary>
/// Represents a logging provider for the application.
/// Implementations define how logging is configured and integrated into the service collection.
/// </summary>
public interface ILoggingProvider
{
    /// <summary>
    /// Configures the logging provider and registers any required services in the specified <see cref="IServiceCollection"/>.
    /// Implementations can use the provided <see cref="LoggingOptions"/> to adjust logging behavior such as log levels, filters, or output formats.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register logging services with.</param>
    /// <param name="options">Configuration options that control the logging behavior.</param>
    void Configure(IServiceCollection services, LoggingOptions options);
}