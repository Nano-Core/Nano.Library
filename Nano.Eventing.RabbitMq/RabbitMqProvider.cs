using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;
using Nano.Eventing.RabbitMq.Extensions;
using System;

namespace Nano.Eventing.RabbitMq;

/// <summary>
/// RabbitMQ implementation of <see cref="IEventingProvider"/>.
/// <para>
/// Configures RabbitMQ-based eventing using EasyNetQ. This includes:
/// <list type="bullet">
/// <item>Registering <see cref="IEventing"/> as <see cref="RabbitMqEventing"/>.</item>
/// <item>Configuring EasyNetQ with connection settings from <see cref="EventingOptions"/>.</item>
/// <item>Setting up JSON serialization using Newtonsoft.Json.</item>
/// <item>Optionally adding RabbitMQ health checks if enabled in <see cref="EventingOptions"/>.</item>
/// </list>
/// </para>
/// </summary>
public abstract class RabbitMqProvider : IEventingProvider
{
    /// <inheritdoc />
    public static void Configure(IServiceCollection services, EventingOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddEasyNetQEventing(options);
    }
}