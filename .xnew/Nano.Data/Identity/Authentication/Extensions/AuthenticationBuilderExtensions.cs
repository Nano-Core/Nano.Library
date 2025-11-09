using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Data.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds external logins to the <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    internal static AuthenticationBuilder AddExternalLogins(this AuthenticationBuilder builder, IdentityOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var googleOptions = options.Authentication.Jwt.ExternalLogins.Google;
        if (googleOptions != null)
        {
            builder
                .AddGoogle(x =>
                {
                    x.ClientId = googleOptions.ClientId;
                    x.ClientSecret = googleOptions.ClientSecret ?? "N/A";

                    foreach (var scope in googleOptions.Scopes)
                    {
                        x.Scope
                            .Add(scope);
                    }
                });
        }

        var facebookOptions = options.Authentication.Jwt.ExternalLogins.Facebook;
        if (options.Authentication.Jwt.ExternalLogins.Facebook != null)
        {
            builder
                .AddFacebook(x =>
                {
                    x.AppId = facebookOptions.AppId;
                    x.AppSecret = facebookOptions.AppSecret;

                    foreach (var scope in facebookOptions.Scopes)
                    {
                        x.Scope
                            .Add(scope);
                    }
                });
        }

        var microsoftOptions = options.Authentication.Jwt.ExternalLogins.Microsoft;
        if (options.Authentication.Jwt.ExternalLogins.Microsoft != null)
        {
            builder
                .AddMicrosoftAccount(x =>
                {
                    x.ClientId = microsoftOptions.ClientId;
                    x.ClientSecret = microsoftOptions.ClientSecret;

                    foreach (var scope in microsoftOptions.Scopes)
                    {
                        x.Scope
                            .Add(scope);
                    }
                });
        }

        return builder;
    }
}