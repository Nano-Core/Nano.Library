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
                .AddOptions<TSection>();

            optionsBuilder
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        if (options is null)
        {
            throw new InvalidOperationException($"Configuration section '{name}' could not be loaded.");
        }

        return services;
    }
}