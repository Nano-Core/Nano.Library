using Serilog.Core;

namespace Nano.Config.Providers.Logging.Interfaces
{
    /// <summary>
    /// Logging Provider interface.
    /// Defines the provider used for logging in the application.
    /// </summary>
    public interface ILoggingProvider : ILogEventSink
    {
        
    }
}