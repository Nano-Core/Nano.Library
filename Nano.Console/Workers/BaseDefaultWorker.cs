using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nano.Models.Eventing.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Console.Workers;

/// <summary>
/// Base Default Worker (abstract).
/// </summary>
public abstract class BaseDefaultWorker : BaseWorker<IRepository>
{
    /// <inheritdoc />
    protected BaseDefaultWorker(ILogger logger, IRepository repository, IEventing eventing, IHostApplicationLifetime applicationLifetime)
        : base(logger, repository, eventing, applicationLifetime)
    {
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}