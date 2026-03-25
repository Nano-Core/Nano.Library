using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication.Abstractions;

/// <summary>
/// Defines a transient authentication repository for handling external login flows,
/// including retrieving available external login providers and generating access tokens for external users.
/// </summary>
/// <remarks>
///     Implementations of this interface are responsible for:
///     <list type="bullet">
///         <item>Retrieving the list of configured external authentication providers (e.g., Google, Facebook, Microsoft).</item>
///         <item>Generating JWT access tokens for users who authenticate via external providers.</item>
///         <item>Supporting both direct external login data and generic external login provider types.</item>
///     </list>
///     This repository is typically used in short-lived authentication scenarios where the user's
///     session is managed via access tokens and transient claims rather than persistent accounts.
/// </remarks>
public interface IAuthTransientRepository
{
    /// <summary>
    /// Retrieves the list of configured external login providers.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="ExternalLoginProvider"/> representing each available provider.</returns>
    Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an external login using direct external login data and generates a corresponding JWT access token.
    /// </summary>
    /// <param name="logInExternalDirect">The external login information, including user details, claims, and roles.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessToken"/> for the authenticated external user.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logInExternalDirect"/> is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if the underlying external repository is not configured.</exception>
    Task<AccessToken> LogInExternalAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an external login using a configured built-in external provider type and generates a corresponding JWT access token.
    /// </summary>
    /// <typeparam name="TProvider">The type of external login provider, derived from <see cref="BaseExternalProvider"/>.</typeparam>
    /// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
    /// <param name="logInExternal">The external login request containing provider-specific information, roles, claims, and options.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessToken"/> for the authenticated external user.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logInExternal"/> is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if the underlying external repository is not configured.</exception>
    /// <exception cref="UnauthorizedException">Thrown if the external login fails or no user is returned.</exception>
    Task<AccessToken> LogInExternalAsync<TProvider, TFlow>(BaseLogInExternal<TProvider, TFlow> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseExternalProvider, new()
        where TFlow : BaseAuthFlow, new();
}