using Nano.Data.Abstractions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Models.Identity.External.Providers;
using Nano.Common.Exceptions;

namespace Nano.App.Web.Identity.Abstractions;

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
    Task<AccessToken> LogInAsync(LogIn logIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalDirect"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalDirectAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}