using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Api.Requests.Interfaces;
using Nano.Api.Responses.Interfaces;

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
        /// <param name="persist">A <see cref="bool"/> indicating if the message should be persisted or not.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task Publish<TMessage>(TMessage body, bool persist = true, CancellationToken cancellationToken = default)
            where TMessage : class;

        /// <summary>
        /// Sends a <see cref="IRequest"/> async, awaiting for <see cref="IResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">Type of <see cref="IRequest"/>.</typeparam>
        /// <typeparam name="TResponse">Type of <see cref="IResponse"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/> to send.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>,</param>
        /// <returns>A Task returning a <see cref="IResponse"/>.</returns>
        Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
            where TResponse : class, IResponse;

        /// <summary>
        /// Consumes messages invoking the passed <paramref name="callback"/> to handle the event.
        /// </summary>
        /// <typeparam name="TMessage">The type of response body.</typeparam>
        /// <param name="callback">The callback to invoke.</param>
        /// <param name="isTemporary">A <see cref="bool"/> determining if a temporary queue is created.</param>
        void Consume<TMessage>(Action<TMessage> callback, bool isTemporary = false)
            where TMessage : class;

        /// <summary>
        /// Recieves a request.
        /// </summary>
        /// <typeparam name="TMessage">The type of response body.</typeparam>
        /// <param name="callback">The callback to invoke.</param>
        void Receive<TMessage>(Action<TMessage> callback)
            where TMessage : class;

        /// <summary>
        /// Response to a recieved request.
        /// </summary>
        /// <typeparam name="TRequest">Type of <see cref="IRequest"/>.</typeparam>
        /// <typeparam name="TResponse">Type of <see cref="IResponse"/>.</typeparam>
        /// <param name="callback">The callback to invoke.</param>
        void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class, IRequest
            where TResponse : class, IResponse;
    }
}