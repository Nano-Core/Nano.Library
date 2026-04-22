using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Extensions;
using Nano.Eventing.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nano.Common;

namespace Nano.Eventing;

internal sealed class RegisterEventingHandlersTask(IEventing eventing) : IRegisterEventingHandlersTask
{
    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    public async Task RegisterEventHandlers(IServiceScope serviceScope, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        var eventHandlerTypes = TypeCache
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
            var eventType = eventHandlerType?
                .GetGenericArguments()[0];

            if (eventType == null)
            {
                continue;
            }

            // BUG: CHAT-GPT Can RoutingKey and OverridePrefetchCount be done in a smarter way? I think we already tried static

            var genericType = typeof(IEventingHandler<>)
                .MakeGenericType(eventType);

            var tempEventHandler = serviceScope.ServiceProvider
                .GetRequiredService(genericType);

            var routingKey = (string?)genericType
                .GetProperty(nameof(IEventingHandler<>.RoutingKey))?
                .GetValue(tempEventHandler);

            var prefetchCount = (ushort?)genericType
                .GetProperty(nameof(IEventingHandler<>.OverridePrefetchCount))?
                .GetValue(tempEventHandler);

            var subscribeMethod = this.eventing
                .GetType()
                .GetMethod(nameof(IEventing.SubscribeAsync));

            subscribeMethod?
                .MakeGenericMethod(eventType)
                .Invoke(this.eventing, [serviceProvider, routingKey, prefetchCount, cancellationToken]);
        }
    }
}