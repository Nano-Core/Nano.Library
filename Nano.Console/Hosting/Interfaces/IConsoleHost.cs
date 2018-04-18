using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Console.Hosting.Interfaces
{
    /// <summary>
    /// Represents a configured console host.
    /// </summary>
    public interface IConsoleHost : IDisposable
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> for the host.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Starts listening on the configured addresses.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts listening on the configured addresses.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempt to gracefully stop the host.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}