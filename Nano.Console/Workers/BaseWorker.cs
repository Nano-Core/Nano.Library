using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Console.Workers
{
    /// <summary>
    /// Base .
    /// </summary>
    public abstract class BaseWorker<TRepository> : IHostedService, IDisposable
        where TRepository : IRepository
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventing Eventing { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected virtual TRepository Repository { get; }

        /// <summary>
        /// Application Lifetime.
        /// </summary>
        protected IHostApplicationLifetime ApplicationLifetime { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        /// <param name="eventing">The <see cref="IEventingProvider"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationLifetime"/>.</param>
        protected BaseWorker(ILogger logger, TRepository repository, IEventing eventing, IHostApplicationLifetime applicationLifetime)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Repository = repository;
            this.Eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));
            this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task StopAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public void Dispose()
        {

        }
    }
}