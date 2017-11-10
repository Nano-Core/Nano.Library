using System;
using EasyNetQ;
using Serilog;

namespace Nano.Eventing.Providers
{
    /// <summary>
    /// EasyNetQ Logger.
    /// </summary>
    public class EasyNetQLogger : IEasyNetQLogger
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public EasyNetQLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.Logger = logger;
        }

        /// <inheritdoc />
        public virtual void DebugWrite(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            this.Logger.Debug(format, args);
        }

        /// <inheritdoc />
        public void InfoWrite(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            this.Logger.Information(format, args);
        }

        /// <inheritdoc />
        public void ErrorWrite(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            this.Logger.Error(format, args);
        }

        /// <inheritdoc />
        public void ErrorWrite(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            this.Logger.Information(exception, exception.Message);
        }
    }
}