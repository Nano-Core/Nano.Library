using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

internal interface IRegisterEventingHandlersTask
{
    Task RegisterEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}