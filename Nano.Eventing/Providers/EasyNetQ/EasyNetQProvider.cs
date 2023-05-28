using System;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="EventingOptions"/>.</param>
    public EasyNetQProvider(EventingOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual IEventing Configure(IServiceCollection services)
    {
        services
            .RegisterEasyNetQ(_ => new ConnectionConfiguration
            {
                Hosts =
                {
                    new HostConfiguration
                    {
                        Host = this.Options.Host,
                        Port = this.Options.Port
                    }
                },
                VirtualHost = this.Options.VHost,
                UserName = this.Options.Username,
                Password = this.Options.Password,
                RequestedHeartbeat = TimeSpan.FromSeconds(this.Options.Heartbeat),
                Timeout = TimeSpan.FromSeconds(this.Options.Timeout)
            });

        var serviceProvider = services
            .BuildServiceProvider();

        var bus = serviceProvider
            .GetRequiredService<IBus>();

        var logger = serviceProvider
            .GetRequiredService<ILogger>();

        return new EasyNetQEventing(bus, logger);
    }
}