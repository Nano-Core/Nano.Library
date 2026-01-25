using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Common.Config.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers and binds a configuration section to a strongly-typed options class, with validation enabled.
    /// </summary>
    /// <typeparam name="TSection">The type of the configuration section class.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the configuration section to.</param>
    /// <param name="name">The name of the configuration section to bind.</param>
    /// <param name="options">Outputs the bound instance of <typeparamref name="TSection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="name"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the configuration section <paramref name="name"/> could not be loaded.</exception>
    public static IServiceCollection AddNanoConfigSection<TSection>(this IServiceCollection services, string name, out TSection options)
        where TSection : class, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(name);

        var section = ConfigManager.Configuration
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