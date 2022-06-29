using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Nano.Security.Extensions;

/// <summary>
/// JwtSecurity Token Extensions.
/// </summary>
public static class JwtSecurityTokenExtensions
{
    /// <summary>
    /// Get a <see cref="Claim"/> encoded in the <see cref="JwtSecurityToken"/>.
    /// </summary>
    /// <param name="jwtSecurityToken">The <see cref="JwtSecurityToken"/>.</param>
    /// <param name="type">The claim type.</param>
    /// <returns>The <see cref="Claim"/>.</returns>
    public static Claim GetClaim(this JwtSecurityToken jwtSecurityToken, string type)
    {
        if (jwtSecurityToken == null)
            throw new ArgumentNullException(nameof(jwtSecurityToken));

        var claim = jwtSecurityToken.Claims
            .FirstOrDefault(x => x.Type == type);

        if (claim == null)
        {
            throw new NullReferenceException(nameof(claim));
        }

        return claim;
    }
}