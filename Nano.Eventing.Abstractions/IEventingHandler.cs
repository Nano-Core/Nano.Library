using System.Threading.Tasks;
using Nano.Eventing.Abstractions.Config;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// Represents a handler for a specific type of event in the Nano eventing system.
/// <para>
///     Implement this interface to process messages of type <typeparamref name="TEvent"/>.
/// </para>
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public interface IEventingHandler<in TEvent>
    where TEvent : class
{
    /// <summary>
    /// Optional override for the prefetch count when subscribing to this event.
    /// If null, the default prefetch count from <see cref="EventingOptions.PrefetchCount"/> is used.
    /// </summary>
    ushort? OverridePrefetchCount { get; }

    /// <summary>
    /// Callback method invoked when a message of type <typeparamref name="TEvent"/> is received.
    /// </summary>
    /// <param name="event">The event instance.</param>
    /// <param name="isRetrying">Indicates whether the message is being redelivered due to a previous failure.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous callback operation.</returns>
    Task CallbackAsync(TEvent @event, bool isRetrying);
}