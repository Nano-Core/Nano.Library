using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nano.Security.Const;
using Nano.Security.Models;

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

            var success = httpContext.Items
                .TryGetValue("IsAnonymous", out var value);

            return success && value != null && (bool)value;
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

            const string PREFIX = "Baerer ";

            var authorizationHeader = httpContext.Request.Headers[HeaderNames.Authorization].ToString();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            if (authorizationHeader.Length <= PREFIX.Length)
            {
                return null;
            }

            var value = authorizationHeader[PREFIX.Length..];

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
            {
                return null;
            }

            var success = Guid.TryParse(value, out var result);

            return success 
                ? result 
                : null;
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

        /// <summary>
        /// Get Jwt External Token.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public static ExternalLoginTokenData GetJwtExternalToken(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var name = httpContext.User
                .FindFirstValue(ClaimTypesExtended.ExternalProviderName);

            var token = httpContext.User
                .FindFirstValue(ClaimTypesExtended.ExternalProviderToken);

            var refreshToken = httpContext.User
                .FindFirstValue(ClaimTypesExtended.ExternalProviderRefreshToken);

            return new ExternalLoginTokenData
            {
                Name = name,
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}