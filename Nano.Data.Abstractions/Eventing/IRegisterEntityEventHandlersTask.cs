using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Eventing;

internal interface IRegisterEntityEventHandlersTask
{
    /// <summary>
    /// Registers entity event handlers using the provided <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used to resolve event handler instances.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the registration operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration operation.</returns>
    Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}