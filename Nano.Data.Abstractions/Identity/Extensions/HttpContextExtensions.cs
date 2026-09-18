using Microsoft.AspNetCore.Http;
using Nano.Data.Abstractions.Identity.Consts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Nano.Data.Abstractions.Identity.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpContext"/> to extract JWT claims and tokens.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the App Id claim from the JWT token in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The App Id as a string, or null if not present.</returns>
    public static string? GetJwtAppId(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return httpContext
            .GetJwtClaimValue(ClaimTypesExtended.AppId);
    }

    /// <summary>
    /// Gets the user identifier ("sub") claim from the JWT token in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The user identifier if present and valid; otherwise, null.</returns>
    public static string? GetJwtUserId(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var value = httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Sub);

        return value;
    }

    /// <summary>
    /// Gets the user identifier ("sub") claim from the JWT token in the HTTP context as a <see cref="Guid"/>.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The user identifier as a <see cref="Guid"/> if present and valid; otherwise, null.</returns>
    public static TIdentity? GetJwtUserId<TIdentity>(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var stringValue = httpContext
            .GetJwtUserId();

        if (stringValue == null)
        {
            return default;
        }

        var value = stringValue
            .ConvertToIdentity<TIdentity>();

        return value;
    }

    /// <summary>
    /// Gets the user name claim ("name") from the JWT token in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The user name as a string, or null if not present.</returns>
    public static string? GetJwtUserName(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Name);
    }

    /// <summary>
    /// Gets the email claim from the JWT token in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The email as a string, or null if not present.</returns>
    public static string? GetJwtUserEmail(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Email);
    }

    /// <summary>
    /// Retrieves the JWT token from the Authorization header of the HTTP request.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>The JWT token string, or null if not present or invalid.</returns>
    public static string? GetJwtToken(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        return httpContext.Request.Headers["Authorization"].ToString()
            .GetJwtToken();
    }

    /// <summary>
    /// Retrieves the value of a specific claim from the JWT token in the HTTP context.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value as a string, or null if the claim or token is not present.</returns>
    public static string? GetJwtClaimValue(this HttpContext httpContext, string claimType)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var jwtToken = httpContext
            .GetJwtToken();

        if (jwtToken == null)
        {
            return null;
        }

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        return jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, claimType);
    }

    /// <summary>
    /// Get Jwt Claim Values.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The claim values, or an empty collection if the token or claim isn't present.</returns>
    public static IEnumerable<string> GetJwtClaimValues(this HttpContext httpContext, string claimType)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        var jwtToken = httpContext.GetJwtToken();
        if (jwtToken == null)
        {
            return [];
        }

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        if (!jwtSecurityTokenHandler.CanReadToken(jwtToken))
        {
            return [];
        }

        return [.. jwtSecurityTokenHandler
            .ReadJwtToken(jwtToken)
            .Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)];
    }
}