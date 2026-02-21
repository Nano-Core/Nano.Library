using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nano.App.ApiClient.Extensions;
using Nano.App.Config;
using Nano.App.Config.Extensions;
using Nano.App.Startup;
using Nano.App.Startup.Abstractions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;

namespace Nano.App.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoApp<TOptions>(this IServiceCollection services, IConfiguration configuration, out TOptions options)
        where TOptions : BaseAppOptions, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddNanoConfig(configuration)
            .AddNanoConfigSection(configuration, BaseAppOptions.SectionName, out options);

        services
            .AddNulLogger()
            .AddStartupTasks()
            .AddNanoApis(options);

        return services;
    }


    private static IServiceCollection AddNulLogger(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<ILogger, NullLogger>(_ => NullLogger.Instance);

        return services;
    }
    private static IServiceCollection AddStartupTasks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<StartupTaskContext>();

        var types = TypesHelper
            .GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(IStartupTask)));

        foreach (var type in types)
        {
            services
                .AddScoped(typeof(IStartupTask), type!);
        }

        services
            .AddHostedService<StartupHostedService>();

        return services;
    }
}