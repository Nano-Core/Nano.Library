using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Nano.Logging
{
    /// <summary>
    /// Logging Options.
    /// Populatd from "Logging" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Log Level.
        /// </summary>
        public virtual LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

        /// <summary>
        /// Log Level Overrides.
        /// </summary>
        public virtual IEnumerable<LogLevelOverride> LogLevelOverrides { get; set; }

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