using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Extensions;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public abstract class BaseAuthIdentityRepository<TIdentity> : IAuthIdentityRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IIdentityRepository<TIdentity> identityRepository;
    private readonly IAuthJwtRepository authJwtRepository;
    private readonly IAuthExternalRepositoryAggregator? authExternalRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="identityRepository">The repository for accessing identity data.</param>
    /// <param name="authJwtRepository">The repository for creating JWT tokens.</param>
    /// <param name="authExternalRepository">Optional external authentication repository (e.g., Google, Facebook, Microsoft).</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityRepository"/> or <paramref name="authJwtRepository"/> is null.</exception>
    protected BaseAuthIdentityRepository(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepositoryAggregator? authExternalRepository = null)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logIn);

        var identityUser = await this.identityRepository
            .SignInAsync(new SignIn
            {
                Username = logIn.Username,
                Password = logIn.Password
            }, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException();
        }

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, logIn.TransientRoles, logIn.TransientClaims, cancellationToken);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logIn.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        accessToken.RefreshToken = logIn.IsRefreshable
            ? await this.CreateRefreshToken(identityUser, logIn.AppId)
            : null;

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalDirect);

        var identityUser = await this.identityRepository
            .SignInExternalAsync(new SignInExternal
            {
                ExternalProvider =
                {
                    Name = logInExternalDirect.ExternalAuthenticationData.ExternalToken.Name,
                    UserId = logInExternalDirect.ExternalAuthenticationData.Id
                }
            }, cancellationToken);

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, logInExternalDirect.TransientRoles, logInExternalDirect.TransientClaims, cancellationToken);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternalDirect.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims,
                ExternalToken = logInExternalDirect.ExternalAuthenticationData.ExternalToken
            });

        accessToken.RefreshToken = logInExternalDirect.IsRefreshable
            ? await this.CreateRefreshToken(identityUser, logInExternalDirect.AppId)
            : null;

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider, TFlow>(BaseLogInExternal<TProvider, TFlow> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseExternalProvider<TFlow>
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(logInExternal);

        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var authenticationData = await this.authExternalRepository
            .AuthenticateAsync(logInExternal.Provider, cancellationToken);

        var claims = logInExternal.TransientClaims
            .Merge(authenticationData.TransientClaims);

        return await this.LogInExternalAsync(new LogInExternalDirect
        {
            AppId = logInExternal.AppId,
            IsRefreshable = logInExternal.IsRefreshable,
            ExternalAuthenticationData = authenticationData,
            TransientRoles = logInExternal.TransientRoles,
            TransientClaims = claims
        }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInRefresh);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var userId = jwtSecurityTokenHandler
            .GetJwtUserId<TIdentity>(logInRefresh.Token);

        var identityUser = await this.identityRepository
            .GetIdentityUserAsync(userId, cancellationToken);

        var appId = jwtSecurityTokenHandler
            .GetJwtAppId(logInRefresh.Token) ?? IdentityDefaults.DEFAULT_APP_ID;

        var identityRefreshToken = await this.identityRepository
            .GetRefreshToken(userId, appId, cancellationToken);

        if (identityRefreshToken == null)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} could not be found.");
        }

        if (identityRefreshToken.Value != logInRefresh.RefreshToken)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} is invalid.");
        }

        if (identityRefreshToken.ExpireAt <= DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} is expired.");
        }

        this.authJwtRepository
            .ValidateTokenForRefresh(logInRefresh.Token);

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, logInRefresh.TransientRoles, logInRefresh.TransientClaims, cancellationToken);

        var externalProviderName = claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderName)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderRefreshToken = claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderRefreshToken)
            .Select(x => x.Value)
            .FirstOrDefault();

        ExternalAuthenticationToken? externalAuthenticationToken = null;
        if (!string.IsNullOrEmpty(externalProviderName) && !string.IsNullOrEmpty(externalProviderRefreshToken))
        {
            if (this.authExternalRepository != null)
            {
                externalAuthenticationToken = await this.authExternalRepository
                    .AuthenticateRefreshAsync(new ExternalProviderGeneric(externalProviderName), externalProviderRefreshToken, cancellationToken);
            }
        }

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = appId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                ExternalToken = externalAuthenticationToken,
                Claims = claims
            });

        accessToken.RefreshToken = await this.CreateRefreshToken(identityUser, appId);

        return accessToken;
    }

    /// <inheritdoc />
    public virtual Task LogOutAsync(TIdentity userId, string appId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(appId);

        return this.identityRepository
            .SignOutAsync(userId, appId, cancellationToken);
    }


    private async Task<RefreshToken> CreateRefreshToken(IdentityUser<TIdentity> identityUser, string? appId = null)
    {
        ArgumentNullException.ThrowIfNull(identityUser);

        var refreshToken = this.authJwtRepository
            .GenerateJwtRefreshToken();

        await this.identityRepository
            .CreateRefreshToken(identityUser.Id, refreshToken, appId ?? IdentityDefaults.DEFAULT_APP_ID);

        return refreshToken;
    }
}