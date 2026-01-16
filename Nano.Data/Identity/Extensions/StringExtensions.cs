using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Nano.Data.Identity.Extensions;

/// <summary>
/// String Extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the <see cref="JwtSecurityToken"/> from a string access token.
    /// </summary>
    /// <param name="accessToken">The access token (jwt).</param>
    /// <returns>The <see cref="JwtSecurityToken"/>.</returns>
    public static JwtSecurityToken GetJwtSecurityToken(this string accessToken)
    {
        ArgumentNullException.ThrowIfNull(accessToken);

        var handler = new JwtSecurityTokenHandler();

        var jsonToken = handler
            .ReadToken(accessToken);

        return jsonToken is not JwtSecurityToken jwtSecurityToken
            ? throw new NullReferenceException(nameof(jwtSecurityToken))
            : jwtSecurityToken;
    }

    /// <summary>
    /// Hmac Encrypt.
    /// </summary>
    /// <param name="apiKey">The unencrtpted generated api-key.</param>
    /// <param name="hmacSecret">The hmac secret</param>
    /// <returns>The encrypted base 64 api-key.</returns>
    public static string HmacEncrypt(this string apiKey, string hmacSecret)
    {
        ArgumentNullException.ThrowIfNull(apiKey);
        ArgumentNullException.ThrowIfNull(hmacSecret);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret));

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        var base64Hash = Convert.ToBase64String(hash);

        return base64Hash;
    }
}