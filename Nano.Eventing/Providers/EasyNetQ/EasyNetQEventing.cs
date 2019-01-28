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
            this.Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <inheritdoc />
        public virtual async Task PublishAsync<TMessage>(TMessage body, string routing = "")
            where TMessage : class
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var name = typeof(TMessage).FullName;
            var message = new Message<TMessage>(body);

            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(name, ExchangeType.Fanout);

            await this.Bus.Advanced
                .PublishAsync(exchange, routing, true, message);
        }

        /// <inheritdoc />
        public virtual async Task SubscribeAsync<TMessage>(Action<TMessage> callback, string routing = "")
            where TMessage : class
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var type = typeof(TMessage);

            var queueName = $"{type.FullName}.{routing}-{Guid.NewGuid()}";
            var queue = await this.Bus.Advanced
                .QueueDeclareAsync($"{queueName}");

            this.Bus.Advanced
                .Consume<TMessage>(queue, (message, info) => callback(message.Body));

            var exchangeName = type.FullName;
            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout);

            this.Bus.Advanced
                .Bind(exchange, queue, routing);
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