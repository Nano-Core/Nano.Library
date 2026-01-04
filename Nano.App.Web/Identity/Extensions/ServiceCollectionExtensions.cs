using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Authentication.Consts;
using Nano.App.Web.Identity.Authentication.Extensions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Nano.App.Config;
using Nano.App.Web.Identity.Authentication;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;
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

        if (options.Jwt != null)
        {
            services
                .AddAuthJwtRepository(options.Jwt)
                .AddAuthRootRepository(options.Jwt.RootLogin)
                .AddAuthExternalRepository(options.Jwt.ExternalLogins)
                .AddAuthExternalFacebookRepository(options.Jwt.ExternalLogins.Facebook)
                .AddAuthExternalGoogleRepository(options.Jwt.ExternalLogins.Google)
                .AddAuthExternalMicrosoftRepository(options.Jwt.ExternalLogins.Microsoft);
        }

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


    private static IServiceCollection AddAuthJwtRepository(this IServiceCollection services, JwtAuthenticationOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthJwtRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new AuthJwtRepository(webOptions.CurrentValue.Identity.Authentication.Jwt);
            });

        return services;
    }
    private static IServiceCollection AddAuthRootRepository(this IServiceCollection services, LogInRootOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthRootRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                var authJwtRepository = x
                    .GetRequiredService<IAuthJwtRepository>();

                return new AuthRootRepository(webOptions.CurrentValue.Identity.Authentication.Jwt.RootLogin, authJwtRepository);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalFacebookRepository(this IServiceCollection services, FacebookOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthExternalFacebookRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new AuthExternalFacebookRepository(webOptions.CurrentValue.Identity.Authentication.Jwt?.ExternalLogins.Facebook);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalGoogleRepository(this IServiceCollection services, GoogleOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthExternalGoogleRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new AuthExternalGoogleRepository(webOptions.CurrentValue.Identity.Authentication.Jwt?.ExternalLogins.Google);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalMicrosoftRepository(this IServiceCollection services, MicrosoftOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthExternalMicrosoftRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new AuthExternalMicrosoftRepository(webOptions.CurrentValue.Identity.Authentication.Jwt?.ExternalLogins.Microsoft);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalRepository(this IServiceCollection services, ExternalLoginOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options is not { IsConfigured: true })
        {
            return services;
        }

        services
            .AddScoped<IAuthExternalRepository>(x =>
            {
                var webOptions = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                var authJwtRepository = x
                    .GetRequiredService<IAuthJwtRepository>();

                var authExternalFacebookRepository = x
                    .GetService<IAuthExternalFacebookRepository>();

                var authExternalGoogleRepository = x
                    .GetService<IAuthExternalGoogleRepository>();

                var authExternalMicrosoftRepository = x
                    .GetService<IAuthExternalMicrosoftRepository>();

                return new AuthExternalRepository(webOptions.CurrentValue.Identity.Authentication.Jwt?.ExternalLogins, authJwtRepository, authExternalFacebookRepository, authExternalGoogleRepository, authExternalMicrosoftRepository);
            });
        services
            .AddScoped<IAuthExternalRepository, AuthExternalRepository>();

        return services;
    }
}