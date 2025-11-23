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
    /// <returns></returns>
    AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identityUser"></param>
    /// <param name="logInRefresh"></param>
    /// <param name="claims"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> GenerateJwtTokenByRefreshAsync(IdentityUser<TIdentity> identityUser, LogInRefresh logInRefresh, IEnumerable<Claim> claims, CancellationToken cancellationToken = default);
}