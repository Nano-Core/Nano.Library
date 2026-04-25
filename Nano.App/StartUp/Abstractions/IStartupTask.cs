using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.StartUp.Abstractions;

/// <summary>
/// Represents a startup task that runs when the application starts and can perform cleanup on stop.
/// </summary>
public interface IStartupTask
{
    /// <summary>
    /// Executes the startup logic of the startup task asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while performing the startup operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous startup operation.</returns>
    Task OnStartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the shutdown or cleanup logic of a startup task asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while performing the stop operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous stop operation.</returns>
    Task OnStopAsync(CancellationToken cancellationToken = default);
}