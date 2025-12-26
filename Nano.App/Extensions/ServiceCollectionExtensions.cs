using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nano.App.Abstractions;
using Nano.App.ApiClient.Extensions;
using Nano.App.StartUp;
using Nano.App.StartUp.Tasks;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;

namespace Nano.App.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoApp<TApplication>(this IServiceCollection services, IConfiguration configuration) 
        where TApplication : class, IApplication
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfig(configuration);

        services
            .AddNulLogger()
            .AddSingleton<IApplication, TApplication>()
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddApis(configuration)
            .AddStartUpTasks();

        return services;
    }


    private static IServiceCollection AddNulLogger(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<ILogger, NullLogger>();

        return services;
    }
    private static IServiceCollection AddStartUpTasks(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<StartupTaskContext>();

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

        return services;
    }
}