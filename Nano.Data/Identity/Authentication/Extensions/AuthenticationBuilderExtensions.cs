using System;
using Microsoft.AspNetCore.Authentication;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Identity.Authentication.Extensions;

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