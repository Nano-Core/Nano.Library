using System;
using Microsoft.AspNetCore.Authentication;
using Nano.App.Web.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Identity.Authentication;

namespace Nano.App.Web.Identity.Authentication.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    internal static AuthenticationBuilder AddApiKeyAuthentication<TIdentity>(this AuthenticationBuilder builder, ApiKeyAuthenticationOptions options)
        where TIdentity : IEquatable<TIdentity>
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
        {
            return builder;
        }

        builder
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler<TIdentity>>(AuthenticationSchemes.API_KEY, _ => { });

        return builder;
    }
}