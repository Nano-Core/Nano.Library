using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Eventing;

/// <summary>
/// Task for initialize entity eventing and registering all entity eventing handlers in the application.
/// </summary>
internal interface IRegisterEntityEventingTask
{
    /// <summary>
    /// Initializes caches for use with entity evening.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the registration operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration operation.</returns>
    Task InitializeEntityEventing(CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers entity event handlers using the provided <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve event handler instances.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the registration operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration operation.</returns>
    Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}