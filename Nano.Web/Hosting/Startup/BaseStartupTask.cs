using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nano.Web.Hosting.Startup
{
    /// <inheritdoc />
    public abstract class BaseStartupTask : IHostedService
    {
        /// <summary>
        /// Startup Task Context.
        /// </summary>
        protected internal virtual StartupTaskContext StartupTaskContext { get; }

        /// <inheritdoc />
        protected BaseStartupTask(StartupTaskContext startupTaskContext)
        {
            this.StartupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
        }

        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}