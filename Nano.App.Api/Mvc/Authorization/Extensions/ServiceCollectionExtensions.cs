using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Mvc.Authentication.Consts;
using System;
using Nano.App.Api.Mvc.Middleware;

namespace Nano.App.Api.Mvc.Authorization.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddSingleton<IAuthorizationMiddlewareResultHandler, OptionalAuthorizationMiddlewareResultHandler>();

        services
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                x.AddPolicy(AuthorizationPolicies.DEFAULT, y =>
                {
                    y.RequireAuthenticatedUser();
                });
            });

        return services;
    }
}