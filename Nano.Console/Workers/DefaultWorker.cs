using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Console.Workers
{
    /// <summary>
    /// Default Worker.
    /// </summary>
    public class DefaultWorker : BaseWorker<IRepository>
    {
        /// <inheritdoc />
        public DefaultWorker(ILogger logger, IRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
