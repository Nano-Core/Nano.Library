using Nano.App.Logging.Providers.Interfaces;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using Serilog.Sinks.RabbitMQ.Sinks.RabbitMQ;

namespace Nano.App.Logging.Providers
{
    /// <summary>
    /// RabbitMQ Logging.
    /// </summary>
    public class RabbitMqLogging : RabbitMQSink, ILoggingProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RabbitMqLogging()
            : base(new RabbitMQConfiguration(), new JsonFormatter(), null)  // TODO: LOGGING: RabbitMq Logging configuratuon
        {

        }
    }
}