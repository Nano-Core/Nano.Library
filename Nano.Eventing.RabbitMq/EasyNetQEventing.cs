using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Abstractions;
using Nano.Eventing.RabbitMq.Extensions;

namespace Nano.Eventing.RabbitMq;

/// <inheritdoc />
public class EasyNetQEventing : IEventing
{
    private const string QUEUE_TYPE = "quorum";

    private readonly IBus bus;
    private readonly ILogger logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="bus">The <see cref="IBus"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EasyNetQEventing(IBus bus, ILogger logger)
    {
        this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public virtual async Task PublishAsync<TMessage>(TMessage body, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        var name = typeof(TMessage).GetFriendlyName();

        var exchange = await this.bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        var message = new Message<TMessage>(body);
        await this.bus.Advanced
            .PublishAsync(exchange, routing, null, null, message, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var name = typeof(TMessage).GetFriendlyName();
        var queueName = EasyNetQEventing.GetQueueName(name, routing);

        var queue = await this.bus.Advanced
            .QueueDeclareAsync(queueName, x =>
            {
                x.AsDurable(true);
                x.AsAutoDelete(false);
                x.AsExclusive(false);
                x.WithQueueType(EasyNetQEventing.QUEUE_TYPE);
            }, cancellationToken);

        var exchange = await this.bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        await this.bus.Advanced
            .BindAsync(exchange, queue, routing, cancellationToken);

        var eventType = typeof(TMessage);
        var genericType = typeof(IEventingHandler<>)
            .MakeGenericType(eventType);

        var prefetchCount = EasyNetQEventing.GetPrefetchCount<TMessage>(serviceProvider, genericType);

        this.bus.Advanced
            .Consume<TMessage>(queue, async (message, info) =>
            {
                try
                {
                    if (info.RoutingKey != routing)
                    {
                        return;
                    }

                    await using var serviceScope = serviceProvider
                        .CreateAsyncScope();

                    var eventHandler = serviceScope.ServiceProvider
                        .GetRequiredService(genericType);

                    var method = eventHandler
                        .GetType()
                        .GetMethod(nameof(IEventingHandler<object>.CallbackAsync));

                    if (method == null)
                        throw new NullReferenceException(nameof(method));

                    var callbackTask = (Task)method
                        .Invoke(eventHandler, 
                        [
                            message.Body,
                            info.Redelivered
                        ]);

                    if (callbackTask == null)
                    {
                        throw new NullReferenceException(nameof(callbackTask));
                    }

                    await callbackTask;
                }
                catch (Exception ex)
                {
                    this.logger
                        .LogError(ex, ex.Message);

                    throw;
                }
            }, x => x
                .WithPrefetchCount(prefetchCount));
    }


    private static string GetQueueName(string name, string routing)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (routing == null)
            throw new ArgumentNullException(nameof(routing));

        var appName = Assembly.GetEntryAssembly()?.GetName().Name;

        var route = string.IsNullOrEmpty(routing)
            ? string.Empty
            : $".{routing}";

        return $"{appName}:{name}{route}";
    }
    private static ushort GetPrefetchCount<TMessage>(IServiceProvider serviceProvider, Type genericType)
        where TMessage : class
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var eventHandlerForPrefetchCount = serviceProvider
            .GetRequiredService(genericType);

        var prefetchCount = (ushort?)genericType
            .GetProperty(nameof(IEventingHandler<TMessage>.OverridePrefetchCount))?
            .GetValue(eventHandlerForPrefetchCount);

        if (!prefetchCount.HasValue)
        {
            var connection = serviceProvider
                .GetRequiredService<ConnectionConfiguration>();

            prefetchCount = connection.PrefetchCount;
        }

        return prefetchCount.Value;
    }
}