namespace Nano.Config.Providers.Eventing.Interfaces
{
    /// <summary>
    /// Event Handler interface.
    /// </summary>
    public interface IEventHandler<in TEvent>
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Callback.
        /// Invoked when recieving a publshed message.
        /// </summary>
        /// <param name="event">The instance of type <typeparamref name="TEvent"/>.</param>
        void Callback(TEvent @event);
    }
}