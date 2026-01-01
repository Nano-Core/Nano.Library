using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Web.Identity.Authentication.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthTransientRepository
{
    /// <summary>
    /// Signs in the admin user statically.
    /// The login is transient, no Identity store is used.
    /// </summary>
    /// <param name="logInRoot">The <see cref="LogInRoot"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> LogInRootTransientAsync(LogInRoot logInRoot);

    /// <summary>
    /// Gets all the configured external logins schemes.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternalTransient">The <see cref="BaseLogInExternal{T}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> LogInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
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
    Task<AccessToken> LogInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalTransientRefreshAsync(LogInExternalTransientRefresh logInRefresh, CancellationToken cancellationToken = default);
}