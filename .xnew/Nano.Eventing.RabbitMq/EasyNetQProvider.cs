using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Abstractions;
using Nano.Eventing.RabbitMq.Extensions;

namespace Nano.Eventing.RabbitMq;

/// <summary>
/// EasyNetQ Provider.
/// </summary>
public class EasyNetQProvider : IEventingProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services
            .AddEasyNetQEventing();
    }
}