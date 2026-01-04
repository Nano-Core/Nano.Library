using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

// TODO: SSO implementation Test and improvements (Facebook, Apple, Google, Microsoft)
// Remember to inject HttpClient

/// <summary>
/// 
/// </summary>
public interface IAuthExternalRepository
{
    /// <summary>
    /// Gets all the configured external logins schemes.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLogInData> GetExternalLogInData<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider;

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternalTransient">The <see cref="BaseLogInExternal{T}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new();

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <param name="externalLogInData">The <see cref="ExternalLogInData"/>.</param>
    /// <param name="transientRoles">The roles added to the token.</param>
    /// <param name="transientClaims">The claims added to the token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> LogInExternalAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLoginTokenData> LogInExternalRefreshAsync(LogInExternalRefresh logInExternalRefresh, CancellationToken cancellationToken = default);
}