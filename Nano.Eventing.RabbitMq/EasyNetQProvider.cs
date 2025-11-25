using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;
using Nano.Eventing.RabbitMq.Extensions;
using System;

namespace Nano.Eventing.RabbitMq;

/// <summary>
/// EasyNetQ Provider.
/// </summary>
public class EasyNetQProvider : IEventingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, EventingOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        services
            .AddEasyNetQEventing(options);
    }
}