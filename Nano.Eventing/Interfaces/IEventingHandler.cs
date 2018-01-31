namespace Nano.Eventing.Interfaces
{
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
        void CallbackAsync(TEvent @event);
    }
}