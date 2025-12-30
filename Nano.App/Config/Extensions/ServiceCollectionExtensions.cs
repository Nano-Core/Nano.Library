using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.App.Config.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoConfig(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        return services
            .AddSingleton(configuration);
    }

    internal static IServiceCollection AddNanoConfigSection<TSection>(this IServiceCollection services, IConfiguration configuration, string name, out TSection options)
        where TSection : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var section = configuration
            .GetSection(name);
        
        options = section
            .Get<TSection>();

        if (options != null)
        {
            var optionsBuilder = services
                .AddOptions<TSection>();

            optionsBuilder
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services;
    }
}