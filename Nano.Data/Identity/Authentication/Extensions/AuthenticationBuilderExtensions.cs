using System;
using Microsoft.AspNetCore.Authentication;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Identity.Authentication.Extensions;

internal static class AuthenticationBuilderExtensions
{
    internal static AuthenticationBuilder AddApiKeyAuthentication<TIdentity>(this AuthenticationBuilder builder, ApiKeyOptions? options)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (options == null)
        {
            return builder;
        }

        builder
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler<TIdentity>>(AuthenticationSchemes.API_KEY, _ => { });

        return builder;
    }
}