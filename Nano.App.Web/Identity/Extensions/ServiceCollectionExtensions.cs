using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App.Web.Identity.Abstractions;
using Nano.App.Web.Identity.Authentication.Consts;
using Nano.App.Web.Identity.Authentication.Extensions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using AuthenticationOptions = Nano.App.Web.Config.AuthenticationOptions;

namespace Nano.App.Web.Identity.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoIdentityAuthentication(this IServiceCollection services, AuthenticationOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap
            .Clear();

        services
            .AddAuthentication(x =>
            {
                x.DefaultScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultAuthenticateScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultChallengeScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultForbidScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultSignInScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultSignOutScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
            })
            .AddJwtAuthentication(options.Jwt)
            .AddPolicyScheme(AuthenticationSchemes.JWT_OR_API_KEY, "JWT or API Key", x =>
            {
                x.ForwardDefaultSelector = context =>
                {
                    var schemeProvider = context.RequestServices
                        .GetRequiredService<IAuthenticationSchemeProvider>();

                    var schemes = schemeProvider
                        .GetAllSchemesAsync()
                        .GetAwaiter()
                        .GetResult()
                        .ToArray();

                    var hasJwt = schemes
                        .Any(s => s.Name == AuthenticationSchemes.JWT);
                    
                    var hasApiKey = schemes
                        .Any(s => s.Name == AuthenticationSchemes.API_KEY);

                    if (hasJwt && context.Request.Headers.ContainsKey(HeaderNames.Authorization))
                    {
                        return AuthenticationSchemes.JWT;
                    }

                    if (hasApiKey && context.Request.Headers.ContainsKey(ApiKeyHeaderNames.X_API_KEY))
                    {
                        return AuthenticationSchemes.API_KEY;
                    }

                    return hasJwt 
                        ? AuthenticationSchemes.JWT 
                        : hasApiKey 
                            ? AuthenticationSchemes.API_KEY 
                            : null;
                };
            });

        services
            .AddScoped<IIdentityJwtRepository, IdentityJwtRepository>()
            .AddScoped<IAuthRepository, DefaultAuthRepository>() // BUG: TIdentity
            .AddScoped<IAuthTransientRepository, DefaultAuthTransientRepository>();

        return services;
    }

    internal static IServiceCollection AddNanoIdentityAuthorization(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                x.AddPolicy(AuthorizationPolicies.DEFAULT, y =>
                {
                    y.RequireAssertion(z =>
                    {
                        var httpContext = z.Resource as HttpContext ?? (z.Resource as AuthorizationFilterContext)?.HttpContext;

                        if (httpContext == null)
                        {
                            return false;
                        }

                        var schemesConfigured = httpContext.RequestServices
                            .GetRequiredService<IAuthenticationSchemeProvider>()
                            .GetAllSchemesAsync()
                            .GetAwaiter()
                            .GetResult();

                        var isAuthConfigured = schemesConfigured
                            .Any(a => a.Name == AuthenticationSchemes.JWT_OR_API_KEY);

                        return !isAuthConfigured || (z.User.Identity?.IsAuthenticated ?? false);
                    });
                });
            });

        return services;
    }
}