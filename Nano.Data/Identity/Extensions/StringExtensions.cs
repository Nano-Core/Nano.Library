using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Nano.Data.Identity.Extensions;

internal static class StringExtensions
{
    internal static JwtSecurityToken GetJwtSecurityToken(this string accessToken)
    {
        ArgumentNullException.ThrowIfNull(accessToken);

        var handler = new JwtSecurityTokenHandler();

        var jsonToken = handler
            .ReadToken(accessToken);

        return jsonToken is not JwtSecurityToken jwtSecurityToken
            ? throw new NullReferenceException(nameof(jwtSecurityToken))
            : jwtSecurityToken;
    }

    internal static string HmacEncrypt(this string apiKey, string hmacSecret)
    {
        ArgumentNullException.ThrowIfNull(apiKey);
        ArgumentNullException.ThrowIfNull(hmacSecret);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret));

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
        var base64Hash = Convert.ToBase64String(hash);

        return base64Hash;
    }
}