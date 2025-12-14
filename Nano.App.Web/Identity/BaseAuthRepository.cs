using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Models.Identity.External.Providers;
using Nano.App.Web.Identity.Models;
using Nano.Common.Exceptions;
using Nano.Common.Identity.Extensions;
using Nano.Data.Abstractions.Identity;
using IdentityOptions = Nano.App.Web.Config.IdentityOptions;

namespace Nano.App.Web.Identity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseAuthRepository<TIdentity> : BaseBaseAuthRepository<TIdentity>, IAuthRepository<TIdentity>
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
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    protected BaseAuthRepository(IdentityOptions options, IIdentityRepository<TIdentity> identityRepository, IIdentityJwtRepository<TIdentity> identityJwtRepository)
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
    public virtual async Task<AccessToken> LogInAsync(LogIn logIn, CancellationToken cancellationToken = default)
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

        var accessToken = this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logIn.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        accessToken.RefreshToken = logIn.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.Authentication.Jwt.RefreshExpirationInHours, logIn.AppId)
            : null;

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
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

        var accessToken = this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternal.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        accessToken.RefreshToken = logInExternal.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.Authentication.Jwt.RefreshExpirationInHours, logInExternal.AppId)
            : null;

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalDirectAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
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

        var accessToken = this.IdentityJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternalDirect.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        accessToken.RefreshToken = logInExternalDirect.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.Authentication.Jwt.RefreshExpirationInHours, logInExternalDirect.AppId)
            : null;

        return accessToken; 
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
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

        var accessToken = await this.IdentityJwtRepository
            .GenerateJwtTokenByRefreshAsync(identityUser, logInRefresh, claims, cancellationToken);

        if (accessToken == null)
        {
            throw new UnauthorizedException();
        }

        accessToken.RefreshToken = await this.IdentityRepository
            .CreateRefreshToken(identityUser, this.Options.Authentication.Jwt.RefreshExpirationInHours, appId);
        
        return accessToken;
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