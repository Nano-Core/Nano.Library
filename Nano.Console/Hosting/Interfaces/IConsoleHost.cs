using System;

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
        /// Starts the console host.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the console host.
        /// </summary>
        void Stop();

        /// <summary>
        /// Initializes the console host.
        /// </summary>
        void Initialize();
    }
}