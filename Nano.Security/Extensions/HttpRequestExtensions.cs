using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Nano.Security.Extensions
{
    /// <summary>
    /// Http Request Extensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Extracts the User from the request header.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>The user.</returns>
        public static string GetUser(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            var header = httpRequest.Headers["Authorization"].FirstOrDefault();

            if (header == null)
                return null;

            var jwtHandler = new JwtSecurityTokenHandler();
            var index = header.IndexOf(" ", 0, StringComparison.Ordinal) + 1;
            var jwtToken = header.Substring(index);

            if (!jwtHandler.CanReadToken(jwtToken))
                return null;

            var jwtSecurityToken = new JwtSecurityToken(jwtToken);

            return jwtSecurityToken.Payload.Sub;
        }
    }
}