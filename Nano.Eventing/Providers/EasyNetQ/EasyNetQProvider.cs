using System;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Interfaces;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
            .RegisterEasyNetQ(this.Options.ConnectionString);

        var serviceProvider = services
            .BuildServiceProvider();

        var bus = serviceProvider
            .GetRequiredService<IBus>();

        var logger = serviceProvider
            .GetRequiredService<ILogger>();

        return new EasyNetQEventing(bus, logger);
    }
}