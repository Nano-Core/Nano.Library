using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.Data.Abstractions.Identity.Authentication.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthRepository : IAuthRepository<Guid>;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public interface IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logIn"></param>
    /// <param name="refreshExpirationInHours"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<AccessToken> LogInAsync(LogIn logIn, int refreshExpirationInHours, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternal"></param>
    /// <param name="refreshExpirationInHours"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, int refreshExpirationInHours, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalDirect"></param>
    /// <param name="refreshExpirationInHours"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalDirectAsync(LogInExternalDirect logInExternalDirect, int refreshExpirationInHours, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInRefresh"></param>
    /// <param name="refreshExpirationInHours"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, int refreshExpirationInHours, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}