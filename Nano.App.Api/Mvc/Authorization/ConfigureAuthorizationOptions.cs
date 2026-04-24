using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Nano.App.Api.Mvc.Authorization.Consts;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.App.Api.Mvc.Authorization;

internal class ConfigureAuthorizationOptions(IAuthenticationSchemeProvider schemes)
    : IConfigureOptions<AuthorizationOptions>
{
    private readonly bool authEnabled = schemes.GetAllSchemesAsync()
        .GetAwaiter()
        .GetResult()
        .Any();

    public void Configure(AuthorizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.FallbackPolicy = null;
        options.InvokeHandlersAfterFailure = false;

        if (this.authEnabled)
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(AuthorizationPolicies.READ, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR,
                    BuiltInUserRoles.WRITER,
                    BuiltInUserRoles.CREATOR,
                    BuiltInUserRoles.EDITOR,
                    BuiltInUserRoles.DELETER,
                    BuiltInUserRoles.READER);
            });

            options.AddPolicy(AuthorizationPolicies.ADD, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR,
                    BuiltInUserRoles.WRITER,
                    BuiltInUserRoles.CREATOR);
            });

            options.AddPolicy(AuthorizationPolicies.EDIT, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR,
                    BuiltInUserRoles.WRITER,
                    BuiltInUserRoles.EDITOR);
            });

            options.AddPolicy(AuthorizationPolicies.ADD_OR_EDIT, y =>
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

            options.AddPolicy(AuthorizationPolicies.DELETE, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR,
                    BuiltInUserRoles.WRITER,
                    BuiltInUserRoles.DELETER);
            });

            options.AddPolicy(AuthorizationPolicies.AUDIT, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR);
            });

            options.AddPolicy(AuthorizationPolicies.IDENTITY, y =>
            {
                y.RequireAuthenticatedUser();

                y.RequireRole(
                    BuiltInUserRoles.ADMINISTRATOR,
                    BuiltInUserRoles.IDENTITY);
            });
        }
        else
        {
            var anonymousPolicy = new AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();

            options.DefaultPolicy = anonymousPolicy;

            options.AddPolicy(AuthorizationPolicies.READ, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.ADD, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.EDIT, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.ADD_OR_EDIT, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.DELETE, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.AUDIT, anonymousPolicy);
            options.AddPolicy(AuthorizationPolicies.IDENTITY, anonymousPolicy);
        }
    }
}