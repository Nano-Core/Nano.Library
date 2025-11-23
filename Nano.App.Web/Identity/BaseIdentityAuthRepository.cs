using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Security.Exceptions;
using Nano.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.App.Web.Identity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class BaseIdentityAuthRepository<TIdentity> : BaseBaseIdentityAuthRepository<TIdentity>, IIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    protected virtual IIdentityRepository<TIdentity> IdentityRepository { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected virtual IIdentityJwtRepository<TIdentity> IdentityJwtRepository { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="Nano.Web.IdentityOptions"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    protected BaseIdentityAuthRepository(IdentityOptions options, IIdentityRepository<TIdentity> identityRepository, IIdentityJwtRepository<TIdentity> identityJwtRepository)
        : base(options)
    {
        this.IdentityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.IdentityJwtRepository = identityJwtRepository ?? throw new ArgumentNullException(nameof(identityJwtRepository));
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        return this.IdentityRepository
            .GetExternalProviderSchemesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityRepository
            .SignInAsync(new SignIn
            {
                Username = logIn.Username,
                Password = logIn.Password,
                IsRememberMe = logIn.IsRememberMe
            }, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException();
        }

        var claims = await this.IdentityRepository
            .GetAllClaims(identityUser, logIn.TransientRoles, logIn.TransientClaims, cancellationToken);

        var jwtToken = await this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken<TIdentity>
            {
                AppId = logIn.AppId,
                UserId = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Claims = claims
            }, cancellationToken);

        var refreshToken = logIn.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, logIn.AppId, this.Options.Authentication.Jwt.RefreshExpirationInHours)
            : null;

        return new AccessToken(jwtToken, refreshToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInExternalDirectAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityRepository
            .SignInExternalAsync(new SignInExternal
            {
                ExternalLogInData = new ExternalLogInData
                {
                    Id = logInExternalDirect.ExternalLogInData.Id,
                    Name = logInExternalDirect.ExternalLogInData.Name,
                    Email = logInExternalDirect.ExternalLogInData.Email,
                    ExternalToken = logInExternalDirect.ExternalLogInData.ExternalToken
                },
                IsRememberMe = logInExternalDirect.IsRememberMe
            }, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException();
        }

        var claims = await this.IdentityRepository
            .GetAllClaims(identityUser, logInExternalDirect.TransientRoles, logInExternalDirect.TransientClaims, cancellationToken);

        var jwtToken = await this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken<TIdentity>
            {
                AppId = logInExternalDirect.AppId,
                UserId = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Claims = claims
            }, cancellationToken);

        var refreshToken = logInExternalDirect.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, logInExternalDirect.AppId, this.Options.Authentication.Jwt.RefreshExpirationInHours)
            : null;

        return new AccessToken(jwtToken, refreshToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternal == null)
            throw new ArgumentNullException(nameof(logInExternal));

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternal.Provider, cancellationToken);

        var identityUser = await this.IdentityRepository
            .SignInExternalAsync(new SignInExternal
            {
                ExternalLogInData = new ExternalLogInData
                {
                    Id = externalLoginData.Id,
                    Name = externalLoginData.Name,
                    Email = externalLoginData.Email,
                    ExternalToken = externalLoginData.ExternalToken
                },
                IsRememberMe = logInExternal.IsRememberMe
            }, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException();
        }

        var claims = await this.IdentityRepository
            .GetAllClaims(identityUser, logInExternal.TransientRoles, logInExternal.TransientClaims, cancellationToken);

        var jwtToken = await this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken<TIdentity>
            {
                AppId = logInExternal.AppId,
                UserId = identityUser.Id,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Claims = claims
            }, cancellationToken);

        var refreshToken = logInExternal.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, logInExternal.AppId, this.Options.Authentication.Jwt.RefreshExpirationInHours)
            : null;

        return new AccessToken(jwtToken, refreshToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> SignInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var userIdString = jwtSecurityTokenHandler
            .GetJwtUserId(logInRefresh.Token);

        var userId = ConvertToIdentity(userIdString);

        var identityUser = await this.IdentityRepository
            .GetIdentityUserAsync(userId, cancellationToken);

        var appId = jwtSecurityTokenHandler
            .GetJwtAppId(logInRefresh.Token);

        var identityRefreshToken = await this.IdentityRepository
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

        var claims = await this.IdentityRepository
            .GetAllClaims(identityUser, logInRefresh.TransientRoles, logInRefresh.TransientClaims, cancellationToken);

        var refreshToken = await this.IdentityRepository
            .CreateRefreshToken(identityUser, appId, this.Options.Authentication.Jwt.RefreshExpirationInHours);

        var accessToken = await this.IdentityJwtRepository
            .GenerateJwtTokenByRefreshAsync(identityUser, logInRefresh, claims, refreshToken, cancellationToken);

        if (accessToken == null)
        {
            throw new UnauthorizedException();
        }

        return new AccessToken(accessToken, refreshToken);
    }

    /// <inheritdoc />
    public virtual Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        return this.IdentityRepository
            .SignOutAsync(cancellationToken);
    }


    private static TIdentity ConvertToIdentity(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var target = typeof(TIdentity);

        if (target == typeof(Guid) && Guid.TryParse(value, out var guid))
        {
            return (TIdentity)(object)guid;
        }

        if (target == typeof(int) && int.TryParse(value, out var integer))
        {
            return (TIdentity)(object)integer;
        }

        if (target == typeof(long) && long.TryParse(value, out var bigInteger))
        {
            return (TIdentity)(object)bigInteger;
        }

        if (target == typeof(string))
        {
            return (TIdentity)(object)value;
        }

        throw new InvalidOperationException($"Unsupported identity type: {target.FullName}");
    }
}