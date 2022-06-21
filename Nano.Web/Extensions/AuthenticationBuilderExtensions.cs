using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nano.Security;

namespace Nano.Web.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds external logins to the <see cref="AuthenticationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="options">The <see cref="SecurityOptions"/>.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    internal static AuthenticationBuilder AddExternalLogins(this AuthenticationBuilder builder, SecurityOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var googleOptions = options.ExternalLogins.Google;
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

        var facebookOptions = options.ExternalLogins.Facebook;
        if (options.ExternalLogins.Facebook != null)
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

        var microsoftOptions = options.ExternalLogins.Microsoft;
        if (options.ExternalLogins.Microsoft != null)
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