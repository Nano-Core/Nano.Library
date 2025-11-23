using Nano.Data.Abstractions.Identity.Models;
using Nano.Security.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Web.Identity.Abstractions;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public interface IIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternalProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLogInData> GetExternalProviderLogInData<TProvider>(TProvider logInExternalProvider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logIn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    Task<AccessToken> SignInAsync(LogIn logIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalDirect"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> SignInExternalDirectAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> SignInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> SignInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}