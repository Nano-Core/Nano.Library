using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Nano.Api.Requests.Interfaces;
using Nano.Api.Responses.Interfaces;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing.Providers
{
    /// <inheritdoc />
    public class EasyNetQEventing : IEventing
    {
        /// <summary>
        /// Bus.
        /// </summary>
        protected virtual IBus Bus { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bus">The <see cref="IBus"/>.</param>
        public EasyNetQEventing(IBus bus)
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));

            this.Bus = bus;
        }

        /// <inheritdoc />
        public virtual async Task Publish<TMessage>(TMessage body, bool persist = true, CancellationToken cancellationToken = default)
            where TMessage : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var name = typeof(TMessage).FullName;
            var message = new Message<TMessage>(body);

            var exchange = this.Bus.Advanced
                .ExchangeDeclare(name, ExchangeType.Fanout, false, persist);

            await this.Bus.Advanced
                .PublishAsync(exchange, string.Empty, true, message);
        }

        /// <inheritdoc />
        public virtual async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Bus
                .RequestAsync<TRequest, TResponse>(request);
        }

        /// <inheritdoc />
        public virtual void Consume<TMessage>(Action<TMessage> callback, bool isTemporary = false)
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TMessage).FullName;
            var exchange = this.Bus.Advanced.ExchangeDeclare(name, ExchangeType.Fanout);
            var queue = !isTemporary
                ? this.Bus.Advanced.QueueDeclare($"{name}")
                : this.Bus.Advanced.QueueDeclare($"{Guid.NewGuid():N}", false, false, false, false, null, 300000);

            this.Bus.Advanced.Consume<TMessage>(queue, (message, info) => callback(message.Body));
            this.Bus.Advanced.Bind(exchange, queue, string.Empty);
        }

        /// <inheritdoc />
        public virtual void Receive<TMessage>(Action<TMessage> callback)
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var name = typeof(TMessage).FullName;

            this.Bus
                .Receive(name, callback);
        }

        /// <inheritdoc />
        public virtual void Respond<TRequest, TResponse>(Func<TRequest, TResponse> callback)
            where TRequest : class, IRequest
            where TResponse : class, IResponse
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            this.Bus
                .Respond(callback);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// Only disposes if passed <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">The <see cref="bool"/> indicating if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Bus?.Dispose();
            }
        }
    }
}