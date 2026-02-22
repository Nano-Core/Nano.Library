using Microsoft.Extensions.DependencyInjection;
using Nano.App.Web.Config;
using System;
using Nano.App.Api.Extensions;

namespace Nano.App.Web.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureNanoWebServices(this IServiceCollection services, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .ConfigureNanoApiServices(options);

        services
            .AddNanoRazor(options)
            .AddNanoBlazor(options);

        return services;
    }

    internal static IServiceCollection AddNanoRazor(this IServiceCollection services, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddRazorPages();

        services
            .AddRazorComponents(x =>
            {
                x.DetailedErrors = options.ErrorHandling.ExposeErrors;
            })
            .AddInteractiveServerComponents();

        return services;
    }

    internal static IServiceCollection AddNanoBlazor(this IServiceCollection services, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddServerSideBlazor(x =>
            {
                x.DetailedErrors = options.ErrorHandling.ExposeErrors;
            });

        return services;
    }
}