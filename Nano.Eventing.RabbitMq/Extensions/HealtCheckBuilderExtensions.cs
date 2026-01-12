using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Eventing.Abstractions.Config;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Nano.Eventing.RabbitMq.Extensions;

internal static class HealtCheckBuilderExtensions
{
    private const string NAME = "rabbitmq";

    internal static IHealthChecksBuilder AddRabbitMqHealthChecks(this IHealthChecksBuilder builder, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder
            .AddRabbitMQ(x =>
            {
                var options = x
                    .GetRequiredService<IOptionsMonitor<EventingOptions>>();

                var connectionString = string.IsNullOrEmpty(options.CurrentValue.Username) || string.IsNullOrEmpty(options.CurrentValue.Password)
                    ? $"amqp://{options.CurrentValue.Host}:{options.CurrentValue.Port}{options.CurrentValue.VHost}"
                    : $"amqp://{options.CurrentValue.Username}:{options.CurrentValue.Password}@{options.CurrentValue.Host}:{options.CurrentValue.Port}{options.CurrentValue.VHost}";

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    AutomaticRecoveryEnabled = true
                };

                return factory
                    .CreateConnectionAsync();
            }, NAME, failureStatus, tags, timeout);

        return builder;
    }
}