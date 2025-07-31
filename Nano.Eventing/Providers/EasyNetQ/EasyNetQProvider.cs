using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Interfaces;
using Nano.Eventing.Providers.EasyNetQ.Extensions;

namespace Nano.Eventing.Providers.EasyNetQ;

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