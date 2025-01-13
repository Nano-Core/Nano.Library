using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Extensions;

namespace Nano.Eventing.Providers.EasyNetQ;

/// <inheritdoc />
public class EasyNetQEventing : IEventing
{
    /// <summary>
    /// Bus.
    /// </summary>
    protected virtual IBus Bus { get; }

    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="bus">The <see cref="IBus"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public EasyNetQEventing(IBus bus, ILogger logger)
    {
        this.Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public virtual async Task PublishAsync<TMessage>(TMessage body, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        var name = typeof(TMessage).GetFriendlyName();

        var exchange = await this.Bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        var message = new Message<TMessage>(body);
        await this.Bus.Advanced
            .PublishAsync(exchange, routing, true, message, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SubscribeAsync<TMessage>(IServiceProvider serviceProvider, string routing = "", CancellationToken cancellationToken = default)
        where TMessage : class
    {
        var name = typeof(TMessage).GetFriendlyName();

        var queueName = this.GetQueueName(name, routing);
        var queue = await this.Bus.Advanced
            .QueueDeclareAsync($"{queueName}", true, false, false, cancellationToken);

        var exchange = await this.Bus.Advanced
            .ExchangeDeclareAsync(name, ExchangeType.Fanout, cancellationToken: cancellationToken);

        await this.Bus.Advanced
            .BindAsync(exchange, queue, routing, cancellationToken);

        var eventType = typeof(TMessage);
        var genericType = typeof(IEventingHandler<>)
            .MakeGenericType(eventType);

        this.Bus.Advanced
            .Consume<TMessage>(queue, async (message, info) =>
            {
                try
                {
                    if (info.RoutingKey != routing)
                    {
                        return;
                    }

                    await using var serviceScope = serviceProvider
                        .CreateAsyncScope();

                    var eventHandler = serviceScope.ServiceProvider
                        .GetRequiredService(genericType);

                    var method = eventHandler
                        .GetType()
                        .GetMethod(nameof(IEventingHandler<object>.CallbackAsync));

                    if (method == null)
                        throw new NullReferenceException(nameof(method));

                    var callbackTask = (Task)method
                        .Invoke(eventHandler, 
                        [
                            message.Body,
                            info.Redelivered
                        ]);

                    if (callbackTask == null)
                    {
                        throw new NullReferenceException(nameof(callbackTask));
                    }

                    await callbackTask;
                }
                catch (Exception ex)
                {
                    this.Logger
                        .LogError(ex, ex.Message);

                    throw;
                }
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

    private string GetQueueName(string name, string routing)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (routing == null)
            throw new ArgumentNullException(nameof(routing));

        var appName = Assembly.GetEntryAssembly()?.GetName().Name;

        var route = string.IsNullOrEmpty(routing)
            ? string.Empty
            : $".{routing}";

        return $"{appName}:{name}{route}";
    }
}