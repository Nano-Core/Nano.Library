using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.Web.Identity.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IIdentityJwtRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateJwtToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JwtToken> GenerateJwtToken(GenerateJwtToken<TIdentity> generateJwtToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessTokenData"></param>
    /// <returns></returns>
    Task<JwtToken> GenerateJwtToken(AccessTokenData<TIdentity> accessTokenData);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identityUser"></param>
    /// <param name="logInRefresh"></param>
    /// <param name="claims"></param>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<JwtToken> GenerateJwtTokenByRefreshAsync(IdentityUser<TIdentity> identityUser, LogInRefresh logInRefresh, IEnumerable<Claim> claims, RefreshToken refreshToken = null, CancellationToken cancellationToken = default);
}