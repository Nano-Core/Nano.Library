using Microsoft.Extensions.Logging;

namespace Nano.Logging.Interfaces;

/// <summary>
/// Logging Provider interface.
/// Defines the provider used for logging in the application.
/// </summary>
public interface ILoggingProvider
{
    /// <summary>
    /// Configures the <see cref="ILoggingProvider"/>.
    /// </summary>
    /// <returns>The <see cref="ILoggerProvider"/>.</returns>
    ILoggerProvider Configure();
}