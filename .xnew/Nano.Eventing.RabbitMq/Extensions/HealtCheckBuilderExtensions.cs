using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Extensions;
using RabbitMQ.Client;

namespace Nano.Eventing.Providers.EasyNetQ.Extensions;

public static class HealtCheckBuilderExtensions
{
    internal static IHealthChecksBuilder AddRabbitMqHealthChecks(this IHealthChecksBuilder builder)
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

        builder
            .AddRabbitMQ(x =>
            {
                var options = x
                    .GetRequiredService<EventingOptions>();

                var connectionString = string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password)
                    ? $"amqp://{options.Host}:{options.Port}{options.VHost}"
                    : $"amqp://{options.Username}:{options.Password}@{options.Host}:{options.Port}{options.VHost}";

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    AutomaticRecoveryEnabled = true
                };

                return factory
                    .CreateConnectionAsync();
            }, "rabbitmq", healthStatus);

        return builder;
    }
}