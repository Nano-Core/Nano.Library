using System;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Interfaces;

namespace Nano.Eventing.Providers.EasyNetQ;

/// <summary>
/// EasyNetQ Provider.
/// </summary>
public class EasyNetQProvider : IEventingProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual EventingOptions Options { get; }

    /// <summary>
    /// Services.
    /// </summary>
    protected virtual IServiceCollection Services { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="EventingOptions"/>.</param>
    public EasyNetQProvider(IServiceCollection services, EventingOptions options)
    {
        this.Services = services ?? throw new ArgumentNullException(nameof(services));
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual IEventing Configure()
    {
        var bus = RabbitHutch.CreateBus(this.Options.ConnectionString, x => x
            .Register(this.Services));

        return new EasyNetQEventing(bus);
    }
}