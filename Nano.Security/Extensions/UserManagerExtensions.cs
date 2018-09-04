using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Nano.Models;

namespace Nano.Security.Extensions
{
    /// <summary>
    /// User Manager Extensions.
    /// </summary>
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Generates the Jwt token.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{T}"/></param>
        /// <param name="user">The <see cref="IdentityUser"/></param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public static async Task<AccessToken> GenerateJwtToken<T>(this UserManager<T> userManager, T user, SecurityOptions options) 
            where T : IdentityUser
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var roles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);
            var roleClaims = roles.Select(y => new Claim(ClaimTypes.Role, y));

            var claims = new Collection<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }
                .Union(userClaims)
                .Union(roleClaims);

            var notBeforeAt = DateTime.UtcNow;
            var expireAt = DateTime.UtcNow.AddHours(options.Jwt.ExpirationInHours);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Jwt.SecretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(options.Jwt.Issuer, options.Jwt.Issuer, claims, notBeforeAt, expireAt, signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AccessToken
            {
                Token = token,
                ExpireAt = expireAt
            };
        }
    }
}