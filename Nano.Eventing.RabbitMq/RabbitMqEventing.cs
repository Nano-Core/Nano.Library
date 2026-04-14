using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Extensions;
using Nano.Eventing.Abstractions;

namespace Nano.Eventing.RabbitMq;

/// <summary>
/// RabbitMQ implementation of <see cref="IEventing"/> using EasyNetQ.
/// <para>
///     Provides a publish/subscribe pattern using fanout exchanges, durable quorum queues,
///     optional routing keys, and message prefetching. Automatically binds exchanges to queues
///     and invokes <see cref="IEventingHandler{TMessage}"/> callbacks when messages are received.
/// </para>
/// </summary>
public sealed class RabbitMqEventing : IEventing
{
    private const string QUEUE_TYPE = "quorum";

    private readonly IBus bus;

    /// <summary>
    /// Initializes a new instance of <see cref="RabbitMqEventing"/> with the specified EasyNetQ bus.
    /// </summary>
    /// <param name="bus">The <see cref="IBus"/> instance for RabbitMQ operations. Cannot be null.</param>
    public RabbitMqEventing(IBus bus)
    {
        this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    /// <inheritdoc />
    public async Task PublishAsync<TMessage>(TMessage body, string? routing = null, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(body);

        var name = typeof(TMessage)
            .GetFriendlyName();

        var exchange = await this.bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        var message = new Message<TMessage>(body);
        await this.bus.Advanced
            .PublishAsync(exchange, routing, null, null, message, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string? routing = null, ushort? prefetchCount = null, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var name = typeof(TMessage)
            .GetFriendlyName();

        var queueName = RabbitMqEventing.GetQueueName(name, routing);

        var queue = await this.bus.Advanced
            .QueueDeclareAsync(queueName, x =>
            {
                x.AsDurable(true);
                x.AsAutoDelete(false);
                x.AsExclusive(false);
                x.WithQueueType(RabbitMqEventing.QUEUE_TYPE);
            }, cancellationToken);

        var exchange = await this.bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        await this.bus.Advanced
            .BindAsync(exchange, queue, routing, cancellationToken);

        await this.bus.Advanced
            .ConsumeAsync<TMessage>(queue, async (message, info, innerCancellatationToken) =>
            {
                await using var serviceScope = serviceProvider
                    .CreateAsyncScope();

                var eventHandler = serviceScope.ServiceProvider
                    .GetRequiredService<IEventingHandler<TMessage>>();

                var callbackTask = eventHandler
                    .GetType()
                    .GetMethod(nameof(IEventingHandler<>.CallbackAsync))?
                    .Invoke(eventHandler, [message.Body, info.Redelivered, innerCancellatationToken]);

                if (callbackTask == null)
                {
                    throw new NullReferenceException(nameof(callbackTask));
                }

                await (Task)callbackTask;
            }, x =>
            {
                if (prefetchCount.HasValue)
                {
                    x.WithPrefetchCount(prefetchCount.Value);
                }
            })
            .ConfigureAwait(false);
    }


    private static string GetQueueName(string name, string? routing = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        var appName = Assembly.GetEntryAssembly()?.GetName().Name;

        var route = string.IsNullOrEmpty(routing)
            ? string.Empty
            : $".{routing}";

        return $"{appName}:{name}{route}";
    }
}