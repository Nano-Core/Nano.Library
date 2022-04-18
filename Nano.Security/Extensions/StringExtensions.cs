using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Nano.Security.Extensions
{
    /// <summary>
    /// String Extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Get Random Token.
        /// Generates and returns a random base64 token.
        /// </summary>
        /// <returns>A random base64 token.</returns>
        public static string GetRandomToken()
        {
            var bytes = new byte[32];
            
            using var generator = RandomNumberGenerator.Create();
           
            generator
                .GetBytes(bytes);

            var token = Convert.ToBase64String(bytes);

            return token;
        }

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
}