using Serilog.Events;

namespace Nano.Logging
{
    /// <summary>
    /// Logging Options.
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "Logging";

        /// <summary>
        /// Log Level.
        /// </summary>
        public virtual LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        /// <summary>
        /// Sinks.
        /// </summary>
        public virtual Sink[] Sinks { get; set; } = new Sink[0];

        /// <summary>
        /// Log Level Overrides.
        /// </summary>
        public virtual LogLevelOverride[] LogLevelOverrides { get; set; } = new LogLevelOverride[0];

        /// <summary>
        /// Sink (nested class).
        /// </summary>
        public class Sink
        {
            /// <summary>
            /// Name.
            /// </summary>
            public virtual string Name { get; set; }

            /// <summary>
            /// Connection String.
            /// </summary>
            public virtual string ConnectionString { get; set; }
        }

        /// <summary>
        /// Log Level Override (nested class).
        /// </summary>
        public class LogLevelOverride
        {
            /// <summary>
            /// Namespace.
            /// </summary>
            public virtual string Namespace { get; set; }

            /// <summary>
            /// Log Level.
            /// </summary>
            public virtual LogEventLevel LogLevel { get; set; } = LogEventLevel.Warning;
        }
    }
}