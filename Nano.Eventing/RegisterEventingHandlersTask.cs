using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nano.Common;

namespace Nano.Eventing;

internal sealed class RegisterEventingHandlersTask(IEventing eventing) : IRegisterEventingHandlersTask
{
    private static readonly MethodInfo subscribeMethod = typeof(IEventing).GetMethod(nameof(IEventing.SubscribeAsync))!;

    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    public async Task RegisterEventHandlers(IServiceScope serviceScope, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        var eventHandlerTypes = TypeCache
            .GetAllTypes()
            .Where(x =>
                x is { IsAbstract: false, IsGenericType: false } &&
                x.GetInterfaces().Any(y =>
                    y.IsGenericType &&
                    y.GetGenericTypeDefinition() == typeof(IEventingHandler<>)))
            .SelectMany(t => t.GetInterfaces()
                .Where(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IEventingHandler<>)))
            .Distinct();

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            var eventType = eventHandlerType
                .GetGenericArguments()[0];

            var routingKey = (string?)eventHandlerType
                .GetProperty(nameof(IEventingHandler.RoutingKey), BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null);

            var overridePrefetchCount = (string?)eventHandlerType
                .GetProperty(nameof(IEventingHandler.OverridePrefetchCount), BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null);

            subscribeMethod
                .MakeGenericMethod(eventType)
                .Invoke(this.eventing, [serviceProvider, routingKey, overridePrefetchCount, cancellationToken]);
        }
    }
}