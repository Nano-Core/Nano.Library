using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Common.Config.Extensions;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSection"></typeparam>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="options"></param>
    /// <returns></returns>
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