using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data.Abstractions.Identity.Extensions;

/// <summary>
/// Http Context Extensions.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Get Jwt App Id.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>The app id.</returns>
    public static string GetJwtAppId(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        return httpContext
            .GetJwtClaimValue(ClaimTypesExtended.AppId);
    }

    /// <summary>
    /// Get Jwt User Name.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>The user name.</returns>
    public static string GetJwtUserName(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        return httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Name);
    }

    /// <summary>
    /// Get Jwt User Email.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>The email.</returns>
    public static string GetJwtUserEmail(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        return httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Email);
    }

    /// <summary>
    /// Get Jwt User Id.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>The user id.</returns>
    public static Guid? GetJwtUserId(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        var value = httpContext
            .GetJwtClaimValue(JwtRegisteredClaimNames.Sub);

        if (value == null)
        {
            return null;
        }

        var success = Guid.TryParse(value, out var result);

        return success
            ? result
            : null;
    }

    /// <summary>
    /// Get Jwt Token.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>The token.</returns>
    public static string GetJwtToken(this HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        const string PREFIX = "Baerer ";

        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return null;
        }

        if (authorizationHeader.Length <= PREFIX.Length)
        {
            return null;
        }

        var value = authorizationHeader[PREFIX.Length..];

        return value == string.Empty
            ? null
            : value;
    }

    /// <summary>
    /// Get Jwt Claim Value.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The email.</returns>
    public static string GetJwtClaimValue(this HttpContext httpContext, string claimType)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

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
}