using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    private readonly IOptionsMonitor<EventingOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{EventingOptions}"/>.</param>
    public EasyNetQProvider(IOptionsMonitor<EventingOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        services
            .AddEasyNetQEventing(options.CurrentValue);
    }
}