using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Nano.Config.Extensions;

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
            .AddOptions()
            .AddSingleton(configuration);
    }

    /// <summary>
    /// Adds a appOptions <see cref="IConfigurationSection"/> as <see cref="IOptions{TOption}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TOption">The option implementation type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The name of the section.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, string name)
        where TOption : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        return services
            .AddConfigOptions<TOption>(name, out _);
    }

    /// <summary>
    /// Adds a appOptions <see cref="IConfigurationSection"/> as <see cref="IOptions{TOption}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TOption">The option implementation type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="name">The name of the section.</param>
    /// <param name="options">The options configured and loaded.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, string name, out TOption options)
        where TOption : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var section = configuration.GetSection(name);

        options = section?.Get<TOption>() ?? new TOption();

        services
            .AddSingleton(options)
            .Configure<TOption>(section);

        return services;
    }

    /// <summary>
    /// Adds a appOptions <see cref="IConfigurationSection"/> as <see cref="IOptions{TOption}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TOption">The option implementation type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <param name="name">The name of the section.</param>
    /// <param name="options">The options configured and loaded.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, IConfiguration configuration, string name, out TOption options)
        where TOption : class, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        if (name == null)
            throw new ArgumentNullException(nameof(name));

        var section = configuration.GetSection(name);
        options = section?.Get<TOption>() ?? new TOption();

        services
            .AddSingleton(options)
            .Configure<TOption>(section);

        return services;
    }
}