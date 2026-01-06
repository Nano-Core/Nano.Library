using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Nano.Common.Extensions;
using Nano.Eventing.Abstractions;

namespace Nano.Eventing.RabbitMq;

/// <inheritdoc />
public class EasyNetQEventing : IEventing
{
    private const string QUEUE_TYPE = "quorum";

    private readonly IBus bus;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="bus">The <see cref="IBus"/>.</param>
    public EasyNetQEventing(IBus bus)
    {
        this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    /// <inheritdoc />
    public virtual async Task PublishAsync<TMessage>(TMessage body, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        var name = typeof(TMessage)
            .GetFriendlyName();

        var exchange = await this.bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        var message = new Message<TMessage>(body);
        await this.bus.Advanced
            .PublishAsync(exchange, routing, null, null, message, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SubscribeAsync<TMessage>(IEventingHandler<TMessage> eventHandler, string routing = "", ushort? prefetchCount = null, CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (eventHandler == null) 
            throw new ArgumentNullException(nameof(eventHandler));

        var name = typeof(TMessage)
            .GetFriendlyName();
        
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

        await this.bus.Advanced
            .ConsumeAsync<TMessage>(queue, (message, info) =>
            {
                var callbackTask = (Task)eventHandler
                    .GetType()
                    .GetMethod(nameof(IEventingHandler<>.CallbackAsync))?
                    .Invoke(eventHandler, [message.Body, info.Redelivered]);

                if (callbackTask == null)
                {
                    throw new NullReferenceException(nameof(callbackTask));
                }

                return callbackTask;
            }, x =>
            {
                if (prefetchCount.HasValue)
                {
                    x.WithPrefetchCount(prefetchCount.Value);
                }
            });
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
}