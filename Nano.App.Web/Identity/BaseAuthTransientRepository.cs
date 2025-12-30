using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Models.Identity.External.Providers;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Abstractions;
using Nano.App.Web.Identity.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Web.Identity;

/// <summary>
/// Base Identity Transient Repository.
/// </summary>
public abstract class BaseAuthTransientRepository : BaseBaseAuthRepository, IAuthTransientRepository
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual IIdentityJwtRepository IdentityJwtRepository { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{WebOptions}"/>.</param>
    /// <param name="identityJwtRepository"></param>
    protected BaseAuthTransientRepository(IOptionsMonitor<WebOptions> options, IIdentityJwtRepository identityJwtRepository)
        : base(options)
    {
        this.IdentityJwtRepository = identityJwtRepository ?? throw new ArgumentNullException(nameof(identityJwtRepository));
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRootTransientAsync(LogInRoot logInRoot)
    {
        if (logInRoot == null)
        {
            throw new ArgumentNullException(nameof(logInRoot));
        }

        if (this.Options.CurrentValue.Identity.Authentication.RootLogin?.Username == null)
        {
            throw new NullReferenceException(nameof(this.Options.CurrentValue.Identity.Authentication.RootLogin.Username));
        }

        if (this.Options.CurrentValue.Identity.Authentication.RootLogin?.Password == null)
        {
            throw new NullReferenceException(nameof(this.Options.CurrentValue.Identity.Authentication.RootLogin.Password));
        }

        if (logInRoot.Username != this.Options.CurrentValue.Identity.Authentication.RootLogin.Username || logInRoot.Password != this.Options.CurrentValue.Identity.Authentication.RootLogin.Password)
        {
            throw new UnauthorizedException($"The user: {logInRoot.Username} failed with incorrect username or password.");
        }

        await Task.CompletedTask;

        var tokenData = new GenerateJwtToken
        {
            UserId = null,
            UserName = this.Options.CurrentValue.Identity.Authentication.RootLogin.Username,
            Claims =
            [
                new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
            ]
        };

        var accessToken = this.IdentityJwtRepository
            .GenerateJwtToken(tokenData);

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var schemes = new List<ExternalLoginProvider>();

        if (this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Facebook != null)
        {
            const string NAME = nameof(this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Facebook);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Google != null)
        {
            const string NAME = nameof(this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Google);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Microsoft != null)
        {
            const string NAME = nameof(this.Options.CurrentValue.Identity.Authentication.Jwt.ExternalLogins.Microsoft);

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
    public virtual async Task<AccessToken> LogInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
            throw new ArgumentNullException(nameof(logInExternalTransient));

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternalTransient.Provider, cancellationToken);

        return await this.LogInExternalTransientAsync(externalLoginData, logInExternalTransient.TransientRoles, logInExternalTransient.TransientClaims, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        await Task.CompletedTask;

        var claims = transientClaims?
            .Select(x => new Claim(x.Key, x.Value)) ?? new List<Claim>();

        var roleClaims = transientRoles?
            .Select(x => new Claim(ClaimTypes.Role, x)) ?? new List<Claim>();

        var tokenData = new GenerateJwtToken
        {
            UserId = externalLogInData.Id,
            UserName = externalLogInData.Name,
            UserEmail = externalLogInData.Email,
            ExternalToken = externalLogInData.ExternalToken,
            Claims = claims
                .Union(roleClaims)
        };

        var accessToken = this.IdentityJwtRepository
            .GenerateJwtToken(tokenData);

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalTransientRefreshAsync(LogInExternalTransientRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        throw new NotImplementedException(); // TODO: External Login Refresh 
    }
}