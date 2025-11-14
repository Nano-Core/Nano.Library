using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nano.Security.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Extensions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Web.Extensions;

namespace Nano.Security;

/// <summary>
/// Base Identity Transient Repository.
/// </summary>
public abstract class BaseIdentityAuthTransientRepository<TIdentity> : BaseIdentityAuthRepository, IIdentityAuthTransientRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Sign In Manager.
    /// </summary>
    protected virtual SignInManager<IdentityUser<TIdentity>> SignInManager { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
    protected BaseIdentityAuthTransientRepository(ILogger logger, Web.IdentityOptions options, SignInManager<IdentityUser<TIdentity>> signInManager)
        : base(logger, options)
    {
        this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    /// <summary>
    /// Signs in the admin user statically.
    /// The login is transient, no Identity store is used.
    /// </summary>
    /// <param name="logIn">The <see cref="LogIn"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<AccessToken> SignInAdminTransientAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        if (logIn == null)
        {
            throw new ArgumentNullException(nameof(logIn));
        }

        if (this.Options.Authentication.RootLogin?.Username == null)
        {
            throw new NullReferenceException(nameof(this.Options.Authentication.RootLogin.Username));
        }

        if (this.Options.Authentication.RootLogin?.Password == null)
        {
            throw new NullReferenceException(nameof(this.Options.Authentication.RootLogin.Password));
        }

        if (logIn.Username != this.Options.Authentication.RootLogin.Username || logIn.Password != this.Options.Authentication.RootLogin.Password)
        {
            throw new UnauthorizedException($"The user: {logIn.Username} failed with incorrect username or password.");
        }

        // BUG: admin (root) user is no longer added to database, it should be pure transient. Just login and make a token
        // Maybe we already do that, but why did we add it to database in dbContext

        var tokenData = new AccessTokenData
        {
            UserId = Guid.NewGuid().ToString(),
            Username = this.Options.Authentication.RootLogin.Username,
            Claims =
            [
                new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
            ]
        };

        var accessToken = this.GenerateJwtToken(tokenData);

        return Task.FromResult(accessToken);
    }

    /// <summary>
    /// Gets all the configured external logins schemes.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await this.SignInManager
            .GetExternalAuthenticationSchemesAsync();

        return schemes
            .Select(x => new ExternalLoginProvider
            {
                Name = x.Name,
                DisplayName = x.DisplayName
            });
    }

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternalTransient">The <see cref="BaseLogInExternal{T}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
        {
            throw new ArgumentNullException(nameof(logInExternalTransient));
        }

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternalTransient.Provider, cancellationToken);

        return await this.SignInExternalTransientAsync(externalLoginData, logInExternalTransient.TransientRoles, logInExternalTransient.TransientClaims, cancellationToken);
    }

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
    public virtual Task<AccessToken> SignInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        var claims = transientClaims?
            .Select(x => new Claim(x.Key, x.Value)) ?? new List<Claim>();

        var roleClaims = transientRoles?
            .Select(x => new Claim(ClaimTypes.Role, x)) ?? new List<Claim>();

        var tokenData = new AccessTokenData
        {
            AppId = DEFAULT_APP_ID,
            UserId = externalLogInData.Id,
            Username = externalLogInData.Name,
            UserEmail = externalLogInData.Email,
            ExternalToken = externalLogInData.ExternalToken,
            Claims = claims
                .Union(roleClaims)
        };

        var jwtToken = this.GenerateJwtToken(tokenData);

        return Task.FromResult(jwtToken);
    }

    /// <summary>
    /// Logs out a user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        var userId = this.SignInManager.Context
            .GetJwtUserId();

        if (userId == null)
        {
            return Task.CompletedTask;
        }

        return this.SignInManager
            .SignOutAsync();
    }
}