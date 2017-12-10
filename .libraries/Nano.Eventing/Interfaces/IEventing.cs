using System;
using System.Threading.Tasks;
using Nano.Eventing.Enums;

namespace Nano.Eventing.Interfaces
{
    /// <summary>
    /// Eventing interface.
    /// </summary>
    public interface IEventing : IDisposable
    {
        /// <summary>
        /// Publishes a message to an exchange using 'fanout' publish/subscribe topology.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message body.</typeparam>
        /// <param name="body">The message body.</param>
        /// <param name="topology"></param>
        /// <param name="routing">The routing key (if any).</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task Publish<TMessage>(TMessage body, Topology topology = Topology.Fanout, string routing = "")
            where TMessage : class;

        /// <summary>
        /// Consumes messages invoking the passed <paramref name="callback"/> to handle the event.
        /// </summary>
        /// <typeparam name="TMessage">The type of response body.</typeparam>
        /// <param name="callback">The callback to invoke.</param>
        /// <param name="topology"></param>
        /// <param name="routing">The routing key (if any).</param>
        void Consume<TMessage>(Action<TMessage> callback, Topology topology = Topology.Fanout, string routing = "")
            where TMessage : class;
    }
}