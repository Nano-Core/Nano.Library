using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Eventing.Abstractions;

internal interface IRegisterEventingHandlersTask
{
    Task RegisterEventHandlers(IServiceScope serviceScope, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}