using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// Event Handler interface.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
public interface IEventingHandler<in TEvent>
    where TEvent : class
{
    /// <summary>
    /// Override Prefetch Count.
    /// </summary>
    ushort? OverridePrefetchCount { get; }

    /// <summary>
    /// CallbackAsync.
    /// Invoked when recieving a publshed message.
    /// </summary>
    /// <param name="event">The instance of type <typeparamref name="TEvent"/>.</param>
    /// <param name="isRetrying">Is Retrying. Indicates whether the message is being redelivered.</param>
    /// <returns>Void.</returns>
    Task CallbackAsync(TEvent @event, bool isRetrying);
}