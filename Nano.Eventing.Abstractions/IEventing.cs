using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

// TODO: Eventing: Kafra Provider
// TODO: Eventing: Azure Service Bus Provider

/// <summary>
/// Represents a generic eventing service for publishing and subscribing to messages.
/// <para>
///     Implementations can use any underlying messaging system (e.g., RabbitMQ, Azure Service Bus, Kafka),
///     and provide consistent methods for sending and receiving strongly-typed messages.
/// </para>
/// </summary>
public interface IEventing
{
    /// <summary>
    /// Publishes a message to an exchange using a 'fanout' publish/subscribe pattern.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message body.</typeparam>
    /// <param name="body">The message body. Cannot be null.</param>
    /// <param name="routing">Optional routing key for selective message delivery.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PublishAsync<TMessage>(TMessage body, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class;

    /// <summary>
    /// Subscribes to messages of type <typeparamref name="TMessage"/> and invokes the provided handler when messages arrive.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to consume.</typeparam>
    /// <param name="eventHandler">The handler implementing <see cref="IEventingHandler{TMessage}"/> to process incoming messages.</param>
    /// <param name="routing">Optional routing key for selective subscription.</param>
    /// <param name="prefetchCount">Optional number of messages to prefetch for performance tuning.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the subscription.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SubscribeAsync<TMessage>(IEventingHandler<TMessage> eventHandler, string routing = "", ushort? prefetchCount = null, CancellationToken cancellationToken = default)
        where TMessage : class;
}