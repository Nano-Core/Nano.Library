using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// Task for registering all event handlers in the application.
/// <para>
///     Scans all loaded types for implementations of <see cref="IEventingHandler{TEvent}"/>,
///     resolves them from the DI container, and subscribes them to the configured <see cref="IEventing"/> instance.
/// </para>
/// </summary>
internal interface IRegisterEventHandlersTask
{
    /// <summary>
    /// Registers all event handlers from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider containing event handler registrations.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the registration process.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration process.</returns>
    Task RegisterEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}