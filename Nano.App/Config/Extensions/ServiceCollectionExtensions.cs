using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config;
using Nano.Common.Config.Extensions;

namespace Nano.App.Config.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoConfig(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton(configuration);
    }

    internal static IServiceCollection AddNanoConfigSection<TSection>(this IServiceCollection services, IConfiguration configuration, string name, out TSection options)
        where TSection : class, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        var section = configuration
            .GetSection(name);

        options = section
            .Get<TSection>() ?? throw new InvalidOperationException($"Configuration section '{name}' could not be loaded.");

        var optionsBuilder = services
            .AddOptions<TSection>();

        optionsBuilder
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}