using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Nano.App.Api.Mvc.Authorization.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IConfigureOptions<AuthorizationOptions>, ConfigureAuthorizationOptions>()
            .AddAuthorization();

        return services;
    }
}