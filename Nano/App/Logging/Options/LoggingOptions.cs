using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Nano.App.Logging.Options
{
    /// <summary>
    /// Logger Options.
    /// Populatd from "Logger" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class LoggingOptions
    {
        /// <summary>
        /// Include Scopes.
        /// </summary>
        public virtual bool IncludeScopes { get; set; } = true;

        /// <summary>
        /// Include Http Context.
        /// </summary>
        public virtual bool IncludeHttpContext { get; set; } = true;

        /// <summary>
        /// Include Request Identifier
        /// </summary>
        public virtual bool IncludeHttpRequestIdentifier { get; set; } = true;

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