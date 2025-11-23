using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Security.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.App.Web.Identity;

/// <summary>
/// Base Identity Transient Repository.
/// </summary>
public abstract class BaseIdentityAuthTransientRepository<TIdentity> : BaseBaseIdentityAuthRepository<TIdentity>, IIdentityAuthTransientRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual IIdentityJwtRepository<TIdentity> IdentityJwtRepository { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="Nano.Web.IdentityOptions"/>.</param>
    /// <param name="identityJwtRepository"></param>
    protected BaseIdentityAuthTransientRepository(IdentityOptions options, IIdentityJwtRepository<TIdentity> identityJwtRepository)
        : base(options)
    {
        this.IdentityJwtRepository = identityJwtRepository ?? throw new ArgumentNullException(nameof(identityJwtRepository));
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInAdminTransientAsync(LogIn logIn, CancellationToken cancellationToken = default)
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

        var tokenData = new AccessTokenData
        {
            UserId = default,
            Username = this.Options.Authentication.RootLogin.Username,
            Claims =
            [
                new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
            ]
        };

        var jwtToken = await this.IdentityJwtRepository
            .GenerateJwtToken(tokenData);

        return new AccessToken(jwtToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var schemes = new List<ExternalLoginProvider>();

        if (this.Options.Authentication.Jwt.ExternalLogins.Facebook != null)
        {
            const string NAME = nameof(this.Options.Authentication.Jwt.ExternalLogins.Facebook);
         
            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.Options.Authentication.Jwt.ExternalLogins.Google != null)
        {
            const string NAME = nameof(this.Options.Authentication.Jwt.ExternalLogins.Google);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.Options.Authentication.Jwt.ExternalLogins.Microsoft != null)
        {
            const string NAME = nameof(this.Options.Authentication.Jwt.ExternalLogins.Microsoft);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        return schemes;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
            throw new ArgumentNullException(nameof(logInExternalTransient));

        // BUG: We are not using all properties from the logInExternalTransient

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternalTransient.Provider, cancellationToken);

        return await this.SignInExternalTransientAsync(externalLoginData, logInExternalTransient.TransientRoles, logInExternalTransient.TransientClaims, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
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
            UserId = externalLogInData.Id,
            Username = externalLogInData.Name,
            UserEmail = externalLogInData.Email,
            ExternalToken = externalLogInData.ExternalToken,
            Claims = claims
                .Union(roleClaims)
        };

        var jwtToken = await this.IdentityJwtRepository
            .GenerateJwtToken(tokenData);

        return new AccessToken(jwtToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInExternalTransientRefreshAsync(LogInTExternalransientRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        throw new NotImplementedException(); // BUG: Implement referesh with external token
    }
}