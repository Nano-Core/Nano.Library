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
    /// Base Worker.
    /// </summary>
    public abstract class BaseWorker : IHostedService, IDisposable
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Application Lifetime.
        /// </summary>
        protected IHostApplicationLifetime ApplicationLifetime { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
        protected BaseWorker(ILogger logger, IHostApplicationLifetime applicationLifetime)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        }

        /// <inheritdoc />
        public abstract Task StartAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            this.ApplicationLifetime
                .StopApplication();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <inheritdoc />
    public abstract class BaseWorker<TRepository> : BaseWorker
        where TRepository : IRepository
    {
        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventing Eventing { get; }

        /// <summary>
        /// Repository.
        /// </summary>
        protected virtual TRepository Repository { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        /// <param name="eventing">The <see cref="IEventingProvider"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationLifetime"/>.</param>
        protected BaseWorker(ILogger logger, TRepository repository, IEventing eventing, IHostApplicationLifetime applicationLifetime)
            : base(logger, applicationLifetime)
        {
            this.Repository = repository;
            this.Eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));
        }
    }
}