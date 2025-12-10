using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Logging.Abstractions.Config;

namespace Nano.Logging.Abstractions;

/// <summary>
/// Logging Provider interface.
/// Defines the provider used for logging in the application.
/// </summary>
public interface ILoggingProvider
{
    /// <summary>
    /// Configures the <see cref="ILoggingProvider"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="LoggingOptions"/>.</param>
    /// <returns>The <see cref="ILoggerProvider"/>.</returns>
    void Configure(IServiceCollection services, LoggingOptions options);
}