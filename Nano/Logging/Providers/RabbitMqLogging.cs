using Nano.Logging.Providers.Interfaces;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using Serilog.Sinks.RabbitMQ.Sinks.RabbitMQ;

namespace Nano.Logging.Providers
{
    // TODO: LOGGING: RabbitMq Logging configuratuon
    /// <summary>
    /// RabbitMQ Logging.
    /// </summary>
    public class RabbitMqLogging : RabbitMQSink, ILoggingProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RabbitMqLogging()
            : base(new RabbitMQConfiguration(), new JsonFormatter(), null) 
        {

        }
    }
}