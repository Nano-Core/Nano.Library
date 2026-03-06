using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.Api.Mvc.Authentication.Extensions;

internal static class AuthenticationBuilderExtensions
{
    internal static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder, JwtAuthenticationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        var rsaSecurityKey = options.PublicKey
            .CreateRsaSecurityKey();

        builder
            .AddJwtBearer(AuthenticationSchemes.JWT, x =>
            {
                x.SaveToken = true;
                x.IncludeErrorDetails = true;
                x.RequireHttpsMetadata = false;

                x.Audience = options.Audience;
                x.ClaimsIssuer = options.Issuer;

                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = rsaSecurityKey,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() != typeof(SecurityTokenExpiredException))
                        {
                            return Task.CompletedTask;
                        }

                        const string KEY = "Token-Expired";

                        context.Response.Headers
                            .TryAdd(KEY, true.ToString());

                        return Task.CompletedTask;
                    }
                };
            })
            .AddExternalLoginFacebook(options.ExternalLogins.Facebook)
            .AddExternalLoginGoogle(options.ExternalLogins.Google)
            .AddExternalLoginMicrosoft(options.ExternalLogins.Microsoft);

        return builder;
    }


    private static AuthenticationBuilder AddExternalLoginFacebook(this AuthenticationBuilder builder, FacebookOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        builder
            .AddFacebook(ExternalLogInProviderNames.FACEBOOK, x =>
            {
                x.AppId = options.AppId;
                x.AppSecret = options.AppSecret;

                foreach (var scope in options.Scopes)
                {
                    x.Scope
                        .Add(scope);
                }
            });

        return builder;
    }
    private static AuthenticationBuilder AddExternalLoginGoogle(this AuthenticationBuilder builder, GoogleOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        builder
            .AddGoogle(ExternalLogInProviderNames.GOOGLE, x =>
            {
                x.ClientId = options.ClientId;
                x.ClientSecret = options.ClientSecret;

                foreach (var scope in options.Scopes)
                {
                    x.Scope
                        .Add(scope);
                }
            });

        return builder;
    }
    private static AuthenticationBuilder AddExternalLoginMicrosoft(this AuthenticationBuilder builder, MicrosoftOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        builder
            .AddMicrosoftAccount(ExternalLogInProviderNames.MICROSOFT, x =>
            {
                x.ClientId = options.ClientId;
                x.ClientSecret = options.ClientSecret;

                foreach (var scope in options.Scopes)
                {
                    x.Scope
                        .Add(scope);
                }
            });

        return builder;
    }
}