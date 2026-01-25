using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data.Abstractions.Identity.Extensions;

/// <summary>
/// Provides extension methods for <see cref="JwtSecurityTokenHandler"/> to extract JWT claims.
/// </summary>
public static class JwtSecurityTokenHandlerExtensions
{
    /// <summary>
    /// Retrieves the user identifier ("sub") claim from a JWT token as a string.
    /// </summary>
    /// <param name="jwtSecurityTokenHandler">The JWT security token handler.</param>
    /// <param name="jwtToken">The JWT token string.</param>
    /// <returns>The user identifier as a string, or null if not present.</returns>
    public static string? GetJwtUserId(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
    {
        ArgumentNullException.ThrowIfNull(jwtSecurityTokenHandler);
        ArgumentNullException.ThrowIfNull(jwtToken);

        return jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, JwtRegisteredClaimNames.Sub);
    }

    /// <summary>
    /// Retrieves the user identifier ("sub") claim from a JWT token and converts it to the specified type.
    /// </summary>
    /// <typeparam name="TIdentity">The target type of the user identifier (e.g., <see cref="Guid"/>, <see cref="int"/>, <see cref="long"/>, <see cref="string"/>).</typeparam>
    /// <param name="jwtSecurityTokenHandler">The JWT security token handler.</param>
    /// <param name="jwtToken">The JWT token string.</param>
    /// <returns>The user identifier converted to <typeparamref name="TIdentity"/>.</returns>
    /// <exception cref="NullReferenceException">Thrown if the claim value is missing.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="TIdentity"/> is unsupported.</exception>
    public static TIdentity GetJwtUserId<TIdentity>(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(jwtSecurityTokenHandler);
        ArgumentNullException.ThrowIfNull(jwtToken);

        var value = jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, JwtRegisteredClaimNames.Sub);

        if (value == null)
        {
            throw new NullReferenceException(nameof(value));
        }

        return value
            .ConvertToTIdentity<TIdentity>();
    }

    /// <summary>
    /// Retrieves the App Id claim from a JWT token as a string.
    /// </summary>
    /// <param name="jwtSecurityTokenHandler">The JWT security token handler.</param>
    /// <param name="jwtToken">The JWT token string.</param>
    /// <returns>The App Id as a string, or null if not present.</returns>
    public static string? GetJwtAppId(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
    {
        ArgumentNullException.ThrowIfNull(jwtSecurityTokenHandler);
        ArgumentNullException.ThrowIfNull(jwtToken);

        return jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, ClaimTypesExtended.AppId);
    }

    /// <summary>
    /// Retrieves the value of a specific claim from a JWT token.
    /// </summary>
    /// <param name="jwtSecurityTokenHandler">The JWT security token handler.</param>
    /// <param name="jwtToken">The JWT token string.</param>
    /// <param name="claimType">The claim type to retrieve.</param>
    /// <returns>The claim value as a string, or null if the claim or token is invalid.</returns>
    public static string? GetClaimValue(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken, string claimType)
    {
        ArgumentNullException.ThrowIfNull(jwtSecurityTokenHandler);
        ArgumentNullException.ThrowIfNull(jwtToken);
        ArgumentNullException.ThrowIfNull(claimType);

        if (!jwtSecurityTokenHandler.CanReadToken(jwtToken))
        {
            return null;
        }

        var securityToken = jwtSecurityTokenHandler
            .ReadJwtToken(jwtToken);

        var value = securityToken.Claims
            .FirstOrDefault(x => x.Type == claimType)?
            .Value;

        return value;
    }


    private static TIdentity ConvertToTIdentity<TIdentity>(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var target = typeof(TIdentity);

        if (target == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return (TIdentity)(object)guid;
        }

        if (target == typeof(int) && int.TryParse(value, out var integer))
        {
            return (TIdentity)(object)integer;
        }

        if (target == typeof(long) && long.TryParse(value, out var bigInteger))
        {
            return (TIdentity)(object)bigInteger;
        }

        if (target == typeof(string))
        {
            return (TIdentity)(object)value;
        }

        throw new InvalidOperationException($"Unsupported identity type: {target.FullName}");
    }
}