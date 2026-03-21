using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using Nano.App.Api.Mvc.Authorization.Consts;
using Nano.Data.Abstractions.Identity.Consts;

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

                x.AddPolicy(AuthorizationPolicies.ADD_OR_EDIT, y =>
                {
                    y.RequireAssertion(z => 
                        z.User.IsInRole(BuiltInUserRoles.ADMINISTRATOR) || 
                        z.User.IsInRole(BuiltInUserRoles.WRITER) || 
                        (
                            z.User.IsInRole(BuiltInUserRoles.CREATOR) && 
                            z.User.IsInRole(BuiltInUserRoles.EDITOR)
                        ));
                });
            });

        return services;
    }
}