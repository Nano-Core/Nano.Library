using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing.Providers.EasyNetQ
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
        public virtual async Task Publish<TMessage>(TMessage body, string routing = "")
            where TMessage : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var name = typeof(TMessage).FullName;
            var message = new Message<TMessage>(body);

            var exchange = this.Bus.Advanced
                .ExchangeDeclare(name, ExchangeType.Fanout);

            await this.Bus.Advanced
                .PublishAsync(exchange, routing, true, message);
        }

        /// <inheritdoc />
        public virtual async Task Subscribe<TMessage>(Action<TMessage> callback, string routing = "")
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            await Task.Run(() =>
            {
                var name = typeof(TMessage).FullName;

                var queue = this.Bus.Advanced
                    .QueueDeclare($"{name}");

                this.Bus.Advanced
                    .Consume<TMessage>(queue, (message, info) => callback(message.Body));

                var exchange = this.Bus.Advanced
                    .ExchangeDeclare(name, ExchangeType.Fanout);

                this.Bus.Advanced
                    .Bind(exchange, queue, routing);
            });
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