using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Authorization.Consts;
using Nano.Data.Abstractions.Identity.Consts;
using System;
using Nano.App.Api.Mvc.Authorization.Requirements;

namespace Nano.App.Api.Mvc.Authorization.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoAuthorization(this IServiceCollection services, AuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<IAuthorizationMiddlewareResultHandler, OptionalAuthorizationMiddlewareResultHandler>()
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                if (options.Jwt == null)
                {
                    x.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddRequirements(new AllowAnonymousRequirement())
                        .Build();
                }
                else
                {
                    x.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    x.AddPolicy(AuthorizationPolicies.ADD_OR_EDIT, y =>
                    {
                        y.RequireAuthenticatedUser();

                        y.RequireAssertion(z =>
                            z.User.IsInRole(BuiltInUserRoles.ADMINISTRATOR) ||
                            z.User.IsInRole(BuiltInUserRoles.WRITER) ||
                            (
                                z.User.IsInRole(BuiltInUserRoles.CREATOR) &&
                                z.User.IsInRole(BuiltInUserRoles.EDITOR)
                            ));
                    });
                }
            });

        return services;
    }
}