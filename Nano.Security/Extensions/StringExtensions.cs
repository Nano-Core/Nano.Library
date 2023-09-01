using System;
using System.IdentityModel.Tokens.Jwt;

namespace Nano.Security.Extensions;

/// <summary>
/// String Extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the <see cref="JwtSecurityToken"/> from a string access token.
    /// </summary>
    /// <param name="accessToken">The access token (jwt)</param>
    /// <returns>The <see cref="JwtSecurityToken"/>.</returns>
    public static JwtSecurityToken GetJwtSecurityToken(this string accessToken)
    {
        if (accessToken == null)
            throw new ArgumentNullException(nameof(accessToken));

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken);

        if (jsonToken is not JwtSecurityToken jwtSecurityToken)
        {
            throw new NullReferenceException(nameof(jwtSecurityToken));
        }

        return jwtSecurityToken;
    }
}