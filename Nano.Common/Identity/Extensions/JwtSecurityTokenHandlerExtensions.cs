using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Web.Extensions;

/// <summary>
/// Jwt Security Token Handler Extensions.
/// </summary>
public static class JwtSecurityTokenHandlerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jwtSecurityTokenHandler"></param>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    public static string GetJwtUserId(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
    {
        if (jwtSecurityTokenHandler == null)
            throw new ArgumentNullException(nameof(jwtSecurityTokenHandler));

        if (jwtToken == null)
            throw new ArgumentNullException(nameof(jwtToken));

        return jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, JwtRegisteredClaimNames.Sub);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jwtSecurityTokenHandler"></param>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    public static string GetJwtAppId(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
    {
        if (jwtSecurityTokenHandler == null)
            throw new ArgumentNullException(nameof(jwtSecurityTokenHandler));

        if (jwtToken == null)
            throw new ArgumentNullException(nameof(jwtToken));

        return jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, ClaimTypesExtended.AppId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jwtSecurityTokenHandler"></param>
    /// <param name="jwtToken"></param>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public static string GetClaimValue(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken, string claimType)
    {
        if (jwtSecurityTokenHandler == null) 
            throw new ArgumentNullException(nameof(jwtSecurityTokenHandler));
        
        if (jwtToken == null) 
            throw new ArgumentNullException(nameof(jwtToken));
        
        if (claimType == null) 
            throw new ArgumentNullException(nameof(claimType));
        
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
}