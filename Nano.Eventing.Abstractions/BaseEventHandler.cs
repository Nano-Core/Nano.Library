using System.Threading;
using System.Threading.Tasks;

namespace Nano.Eventing.Abstractions;

/// <summary>
/// Represents the base class for a handler for a specific type of event in the Nano eventing system.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public abstract class BaseEventHandler<TEvent> : IEventingHandler<TEvent>
    where TEvent : class
{
    /// <inheritdoc />
    public abstract Task CallbackAsync(TEvent @event, bool isRedelivered, CancellationToken cancellationToken = default);
}