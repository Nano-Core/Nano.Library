using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Eventing.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing;

internal sealed class RegisterEventHandlersTask(IEventing eventing) : IRegisterEventHandlersTask
{
    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    /// <inheritdoc />
    public async Task RegisterEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        var eventHandlerTypes = TypesHelper
            .GetAllTypes()
            .SelectMany(x => x.GetInterfaces(), (x, y) => new
            {
                Type = x,
                InterfaceType = y
            })
            .Where(x =>
                x.Type is { IsAbstract: false, IsGenericType: false } &&
                x.Type.IsTypeOf(typeof(IEventingHandler<>)))
            .GroupBy(x => new
            {
                TypeName = x.Type.FullName,
                InterfaceTypeName = x.InterfaceType.FullName
            })
            .Select(x => x.FirstOrDefault()?.InterfaceType)
            .Where(x => x != null);

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            var routing = string.Empty;

            var eventType = eventHandlerType?
                .GetGenericArguments()[0];

            if (eventType == null)
            {
                continue;
            }

            var genericType = typeof(IEventingHandler<>)
                .MakeGenericType(eventType);

            var eventHandler = serviceProvider
                .GetRequiredService(genericType);

            var prefetchCount = (ushort?)genericType
                .GetProperty(nameof(IEventingHandler<>.OverridePrefetchCount))?
                .GetValue(eventHandler);

            var subscribeMethod = eventing
                .GetType()
                .GetMethod(nameof(IEventing.SubscribeAsync));

            subscribeMethod?
                .MakeGenericMethod(eventType)
                .Invoke(eventing, [eventHandler, routing, prefetchCount, CancellationToken.None]);
        }
    }
}