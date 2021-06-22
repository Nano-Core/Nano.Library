using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Nano.Security.Const;

namespace Nano.Security.Extensions
{
    /// <summary>
    /// Http Context Extensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Get Is Anonymous.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>Whehter the current action is anonymous.</returns>
        public static bool GetIsAnonymous(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            return (bool)httpContext.Items["IsAnonymous"];
        }

        /// <summary>
        /// Get Jwt App Id.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>The app id.</returns>
        public static string GetJwtAppId(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var value = httpContext.User
                .FindFirstValue(ClaimTypesExtended.AppId);

            return value;
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

            var value = httpContext
                .GetTokenAsync("access_token")
                .Result;

            return value;
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

            var value = httpContext.User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (value == null)
                return null;

            var success = Guid.TryParse(value, out var result);

            return success 
                ? result 
                : (Guid?)null;
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

            var value = httpContext.User
                .FindFirstValue(JwtRegisteredClaimNames.Name);

            return value;
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

            var value = httpContext.User
                .FindFirstValue(JwtRegisteredClaimNames.Email);

            return value;
        }
    }
}