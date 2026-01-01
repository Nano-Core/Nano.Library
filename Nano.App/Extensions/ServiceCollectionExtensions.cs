using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nano.App.ApiClient.Extensions;
using Nano.App.Config;
using Nano.App.Config.Extensions;
using Nano.App.StartUp;
using Nano.App.StartUp.Tasks;
using Nano.Common.Extensions;
using Nano.Common.Helpers;

namespace Nano.App.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoApp<TOptions>(this IServiceCollection services, IConfiguration configuration, out TOptions options)
        where TOptions : BaseAppOptions, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddNanoConfig(configuration)
            .AddNanoConfigSection(configuration, BaseAppOptions.SectionName, out options);

        services
            .AddNulLogger()
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddStartUpTasks()
            .AddNanoApis(options);

        return services;
    }


    private static IServiceCollection AddNulLogger(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<ILogger, NullLogger>(_ => NullLogger.Instance);

        return services;
    }
    private static IServiceCollection AddStartUpTasks(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<StartupTaskContext>();

        TypesHelper
            .GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseStartupTask)))
            .ToList()
            .ForEach(x =>
            {
                services
                    .AddSingleton(typeof(IHostedService), x);
            });

        return services;
    }
}