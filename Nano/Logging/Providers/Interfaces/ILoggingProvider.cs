using Serilog.Core;

namespace Nano.Logging.Providers.Interfaces
{
    /// <summary>
    /// ogging Provider.
    /// Defines a provider for logging in the application.
    /// </summary>
    public interface ILoggingProvider : ILogEventSink
    {
        
    }
}