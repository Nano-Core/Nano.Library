using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.Api.Identity.Authentication.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    internal static AuthenticationBuilder AddJwtAuthentication(this AuthenticationBuilder builder, JwtAuthenticationOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

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
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            const string KEY = "Token-Expired";

                            context.Response.Headers
                                .TryAdd(KEY, true.ToString());
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddExternalLoginGoogle(options.ExternalLogins.Google)
            .AddExternalLoginFacebook(options.ExternalLogins.Facebook)
            .AddExternalLoginMicrosoft(options.ExternalLogins.Microsoft);

        return builder;
    }


    private static AuthenticationBuilder AddExternalLoginGoogle(this AuthenticationBuilder builder, GoogleOptions options)
    {
        if (builder == null) 
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        builder
            .AddGoogle("Google", x =>
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
    private static AuthenticationBuilder AddExternalLoginFacebook(this AuthenticationBuilder builder, FacebookOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        builder
            .AddFacebook("Facebook", x =>
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
    private static AuthenticationBuilder AddExternalLoginMicrosoft(this AuthenticationBuilder builder, MicrosoftOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        builder
            .AddMicrosoftAccount("Microsoft", x =>
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