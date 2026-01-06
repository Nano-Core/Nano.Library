using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data.Abstractions.Identity.Extensions;

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
    public static TIdentity GetJwtUserId<TIdentity>(this JwtSecurityTokenHandler jwtSecurityTokenHandler, string jwtToken)
    {
        if (jwtSecurityTokenHandler == null)
            throw new ArgumentNullException(nameof(jwtSecurityTokenHandler));

        if (jwtToken == null)
            throw new ArgumentNullException(nameof(jwtToken));

        var value = jwtSecurityTokenHandler
            .GetClaimValue(jwtToken, JwtRegisteredClaimNames.Sub);

        return value
            .ConvertToTIdentity<TIdentity>();
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


    private static TIdentity ConvertToTIdentity<TIdentity>(this string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var target = typeof(TIdentity);

        if (target == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return (TIdentity)(object)guid;
        }

        if (target == typeof(int) && int.TryParse(value, out var integer))
        {
            return (TIdentity)(object)integer;
        }

        if (target == typeof(long) && long.TryParse(value, out var bigInteger))
        {
            return (TIdentity)(object)bigInteger;
        }

        if (target == typeof(string))
        {
            return (TIdentity)(object)value;
        }

        throw new InvalidOperationException($"Unsupported identity type: {target.FullName}");
    }
}