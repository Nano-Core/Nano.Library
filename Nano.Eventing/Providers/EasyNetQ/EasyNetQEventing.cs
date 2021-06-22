using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;

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

            var name = typeof(TMessage).GetFriendlyName();
            var message = new Message<TMessage>(body);

            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(name, ExchangeType.Fanout);

            await this.Bus.Advanced
                .PublishAsync(exchange, routing, true, message);
        }

        /// <inheritdoc />
        public virtual async Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string routing = "", CancellationToken cancellationToken = default)
            where TMessage : class
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var name = typeof(TMessage).GetFriendlyName();
            var route = string.IsNullOrEmpty(routing) ? string.Empty : $".{routing}";
            var appName = Assembly.GetEntryAssembly()?.GetName().Name;
            var queueName = $"{appName}:{name}{route}";

            var queue = await this.Bus.Advanced
                .QueueDeclareAsync($"{queueName}", cancellationToken);

            var exchange = await this.Bus.Advanced
                .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

            await this.Bus.Advanced
                .BindAsync(exchange, queue, routing, cancellationToken);

            this.Bus.Advanced
                .Consume<TMessage>(queue, (message, info) =>
                {
                    if (info.RoutingKey != routing)
                        return;

                    using var serviceScope = serviceProvider.CreateScope();
                   
                    var eventType = message.MessageType;
                    var genericType = typeof(IEventingHandler<>).MakeGenericType(eventType);
                    var eventHandler = serviceScope.ServiceProvider.GetRequiredService(genericType);

                    var method = eventHandler
                        .GetType()
                        .GetMethod("CallbackAsync");

                    if (method == null)
                        throw new NullReferenceException(nameof(method));

                    var callbackTask = (Task)method
                        .Invoke(eventHandler, new object[] { message.Body });

                    callbackTask?
                        .Wait(cancellationToken);
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