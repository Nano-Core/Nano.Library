using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Console.Workers
{
    /// <summary>
    /// Default Hosted Service.
    /// </summary>
    public class DefaultWorker : BaseWorker<IService>
    {
        /// <inheritdoc />
        public DefaultWorker(ILogger logger, IService service, IEventing eventing)
            : base(logger, service, eventing)
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
