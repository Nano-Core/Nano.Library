using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Nano.App.Abstractions;
using Nano.App.ApiClient.Extensions;
using Nano.App.Config;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Common.Startup;
using Nano.Common.Startup.Tasks;

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
    internal static IServiceCollection AddApp<TApplication>(this IServiceCollection services, IConfiguration configuration) 
        where TApplication : class, IApplication
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddSingleton<IApplication, TApplication>();

        services
            .AddConfigSection<AppOptions>(AppOptions.SectionName, out _);

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
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<StartupTaskContext>();

        services
            .AddConfig(configuration) // BUG: Haven't we alrady added this?
            .AddApis(configuration);

        return services;
    }
}