using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.Security;

/// <summary>
/// IdentityManager interface.
/// </summary>
public interface IIdentityAuthTransientRepository<TIdentity> : IIdentityRepository
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Signs in the admin user statically.
    /// The login is transient, no Identity store is used.
    /// </summary>
    /// <param name="logIn">The <see cref="LogIn"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    Task<AccessToken> SignInAdminTransientAsync(LogIn logIn, CancellationToken cancellationToken = default);

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
    Task<AccessToken> SignInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
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
    Task<AccessToken> SignInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}