using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Console.Workers.Abstractions;

/// <summary>
/// Defines a contract for a worker that can be started and stopped asynchronously by the application host.
/// </summary>
public interface IWorker
{
    /// <summary>
    /// Performs the asynchronous startup logic for the worker.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the startup operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous startup operation.</returns>
    Task OnStartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs the asynchronous shutdown logic for the worker.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the stop operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous stop operation.</returns>
    Task OnStopAsync(CancellationToken cancellationToken = default);
}