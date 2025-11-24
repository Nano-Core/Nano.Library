using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Helpers;

namespace Nano.Common.Config.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IConfiguration"/> appOptions to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        return services
            .AddSingleton(configuration);
    }

    /// <summary>
    /// Adds a appOptions <see cref="IConfigurationSection"/>.
    /// </summary>
    /// <typeparam name="TSection">The option implementation type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The name of the section.</param>
    /// <param name="options">The options configured and loaded.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfigSection<TSection>(this IServiceCollection services, string name, out TSection options)
        where TSection : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var section = ConfigManager.Configuration
            .GetSection(name);
        
        options = section
            .Get<TSection>();

        if (options != null)
        {
            var optionsBuilder = services
                .AddOptions<TSection>(name);

            optionsBuilder
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        return services;
    }
}