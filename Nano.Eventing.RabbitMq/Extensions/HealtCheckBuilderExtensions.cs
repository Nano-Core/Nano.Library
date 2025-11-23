using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Eventing.Abstractions.Config;
using Nano.Models.Extensions;
using System;
using System.Collections.Generic;

namespace Nano.Eventing.RabbitMq.Extensions;

internal static class HealtCheckBuilderExtensions
{
    private const string NAME = "rabbitmq";

    internal static IHealthChecksBuilder AddRabbitMqHealthChecks(this IHealthChecksBuilder builder, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var eventingOptions = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<EventingOptions>();

        if (!eventingOptions.UseHealthCheck)
        {
            return builder;
        }

        var healthStatus = eventingOptions.UnhealthyStatus
            .GetHealthStatus();

        var connectionString = string.IsNullOrEmpty(eventingOptions.Username) || string.IsNullOrEmpty(eventingOptions.Password)
            ? $"amqp://{eventingOptions.Host}:{eventingOptions.Port}{eventingOptions.VHost}"
            : $"amqp://{eventingOptions.Username}:{eventingOptions.Password}@{eventingOptions.Host}:{eventingOptions.Port}{eventingOptions.VHost}";

        builder
            .AddRabbitMQ(connectionString, null, NAME, healthStatus, tags, timeout);

        // BUG: Upgrade to latest AspNetCore.HealthChecks.Rabbitmq, but we can't because eventing fails as the EasyNetQ expects a lower version of RabbitMQ.Client.
        //builder
        //    .AddRabbitMQ(x =>
        //    {
        //        var options = x
        //            .GetRequiredService<EventingOptions>();

        //        var connectionString = string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password)
        //            ? $"amqp://{options.Host}:{options.Port}{options.VHost}"
        //            : $"amqp://{options.Username}:{options.Password}@{options.Host}:{options.Port}{options.VHost}";

        //        var factory = new ConnectionFactory
        //        {
        //            Uri = new Uri(connectionString),
        //            AutomaticRecoveryEnabled = true
        //        };

        //        return factory
        //            .CreateConnectionAsync();
        //    }, NAME, healthStatus, tags, timeout);

        return builder;
    }
}