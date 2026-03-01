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
    public string? RoutingKey { get; set; }

    /// <inheritdoc />
    public ushort? OverridePrefetchCount { get; set; }

    /// <summary>
    /// Initializes a new instance of a derived <see cref="BaseEventHandler{T}"/>.
    /// </summary>
    /// <param name="routingKey">The routing key. Setting the <see cref="RoutingKey"/>.</param>
    /// <param name="overridePrefetchCount">The prefecth count to override configuration. Setting the <see cref="OverridePrefetchCount"/>.</param>
    protected BaseEventHandler(string? routingKey = null, ushort? overridePrefetchCount = null)
    {
        this.RoutingKey = routingKey;
        this.OverridePrefetchCount = overridePrefetchCount;
    }

    /// <inheritdoc />
    public abstract Task CallbackAsync(TEvent @event, bool isRedelivered);
}