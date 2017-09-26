using System;
using Microsoft.Extensions.Logging;

namespace Nano.App.Logging.Extensions
{
    /// <summary>
    /// Logging Builder Extensions.
    /// </summary>
    public static class LoggingBuilderExtensions
    {
        /// <summary>
        /// Adds logging provider for <see cref="ILoggerProvider"/> to the <see cref="ILoggingBuilder"/>.
        /// </summary>
        /// <typeparam name="TLogging">The <see cref="ILoggerProvider"/> implementation.</typeparam>
        /// <param name="builder">The <see cref="ILoggingBuilder"/>.</param>
        /// <returns>The <see cref="ILoggingBuilder"/>.</returns>
        public static ILoggingBuilder AddProvider<TLogging>(this ILoggingBuilder builder)
            where TLogging : ILoggerProvider, new()
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder
                .AddProvider(new TLogging());
        }
    }
}