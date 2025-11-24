using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Eventing.Abstractions.Config;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Nano.Eventing.RabbitMq.Extensions;

// TODO: Upgrade to latest AspNetCore.HealthChecks.Rabbitmq, but we can't because eventing fails as the EasyNetQ expects a lower version of RabbitMQ.Client.

internal static class HealtCheckBuilderExtensions
{
    private const string NAME = "rabbitmq";

    internal static IHealthChecksBuilder AddRabbitMqHealthChecks(this IHealthChecksBuilder builder, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder
            .AddRabbitMQ((x, y) =>
            {
                var options = x
                    .GetRequiredService<IOptionsMonitor<EventingOptions>>();

                var connectionString = string.IsNullOrEmpty(options.CurrentValue.Username) || string.IsNullOrEmpty(options.CurrentValue.Password)
                    ? $"amqp://{options.CurrentValue.Host}:{options.CurrentValue.Port}{options.CurrentValue.VHost}"
                    : $"amqp://{options.CurrentValue.Username}:{options.CurrentValue.Password}@{options.CurrentValue.Host}:{options.CurrentValue.Port}{options.CurrentValue.VHost}";

                y.RequestedConnectionTimeout = timeout;
                y.ConnectionFactory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    AutomaticRecoveryEnabled = true
                };
            }, NAME, failureStatus, tags, timeout);

        return builder;
    }
}