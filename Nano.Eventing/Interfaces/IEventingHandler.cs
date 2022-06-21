using System.Threading.Tasks;

namespace Nano.Eventing.Interfaces;

/// <summary>
/// Event Handler interface.
/// </summary>
public interface IEventingHandler<in TEvent>
    where TEvent : class
{
    /// <summary>
    /// CallbackAsync.
    /// Invoked when recieving a publshed message.
    /// </summary>
    /// <param name="event">The instance of type <typeparamref name="TEvent"/>.</param>
    /// <param name="isRetrying">Is Retrying. Indicates whether the message is being redelivered.</param>
    /// <returns>Void.</returns>
    Task CallbackAsync(TEvent @event, bool isRetrying);
}