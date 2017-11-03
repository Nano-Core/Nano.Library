using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Nano.Config
{
    /// <summary>
    /// Logging Options.
    /// Populated from "Logging" <see cref="IConfigurationSection"/>.
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
        /// Log Level Overrides.
        /// </summary>
        public virtual IEnumerable<LogLevelOverride> LogLevelOverrides { get; set; } = new List<LogLevelOverride>();

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