using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Api;
using Nano.App.Startup;
using Nano.App.Startup.Tasks;
using Nano.Config.Extensions;
using Nano.Models.Extensions;
using Nano.Models.Helpers;

namespace Nano.App.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="AppOptions"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfigOptions<AppOptions>(configuration, AppOptions.SectionName, out _);

        TypesHelper.GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseStartupTask)))
            .ToList()
            .ForEach(x =>
            {
                services
                    .AddSingleton(typeof(IHostedService), x);
            });

        services
            .AddSingleton<IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>()
            .AddSingleton<StartupTaskContext>()
            .AddHostedService<InitializeApplicationStartupTask>();

        return services;
    }

    /// <summary>
    /// Add Apis to the <see cref="IServiceCollection"/>..
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddApis(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var hosts = new List<string>();

        TypesHelper.GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseApi)))
            .Distinct()
            .ToList()
            .ForEach(x =>
            {
                var section = configuration.GetSection(x.Name);
                var options = section.Get<ApiOptions>();

                if (options == null)
                {
                    return;
                }

                var instance = Activator.CreateInstance(x, options);

                if (instance == null)
                {
                    return;
                }

                services
                    .AddSingleton(x, instance);

                if (hosts.Contains(options.Host))
                {
                    return;
                }

                if (options.UseHealthCheck)
                {
                    services
                        .AddHealthChecks()
                        .AddTcpHealthCheck(y => y.AddHost(options.Host, options.Port), options.Host, options.UnhealthyStatus);
                }

                hosts
                    .Add(options.Host);
            });

        return services;
    }
}