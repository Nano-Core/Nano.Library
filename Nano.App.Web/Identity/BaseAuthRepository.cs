using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Models.Identity.External.Providers;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Models;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Exceptions;
using Nano.Data.Abstractions.Identity.Extensions;

namespace Nano.App.Web.Identity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseAuthRepository<TIdentity> : BaseBaseAuthRepository, IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    protected virtual IIdentityRepository<TIdentity> IdentityRepository { get; }
    
    /// <summary>
    /// 
    /// </summary>
    protected virtual IIdentityJwtRepository IdentityJwtRepository { get; } // BUG: TIdentity: should is the problem, that it needs to be moved also

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{WebOptions}"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    protected BaseAuthRepository(IOptionsMonitor<WebOptions> options, IIdentityRepository<TIdentity> identityRepository, IIdentityJwtRepository identityJwtRepository)
        : base(options)
    {
        this.IdentityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.IdentityJwtRepository = identityJwtRepository ?? throw new ArgumentNullException(nameof(identityJwtRepository));
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
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

        var refreshTokenExpiry = logIn.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, logIn.AppId)
            : null;

        if (refreshTokenExpiry != null)
        {
            accessToken.RefreshToken = new RefreshToken
            {
                Token = refreshTokenExpiry.Value,
                ExpireAt = refreshTokenExpiry.ExpireAt
            };
        }

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
                ExternalProvider =
                {
                    LoginProvider = externalLoginData.ExternalToken.Name,
                    ProviderKey = externalLoginData.Id
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

        var refreshTokenExpiry = logInExternal.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, logInExternal.AppId)
            : null;

        if (refreshTokenExpiry != null)
        {
            accessToken.RefreshToken = new RefreshToken
            {
                Token = refreshTokenExpiry.Value,
                ExpireAt = refreshTokenExpiry.ExpireAt
            };
        }

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalDirectAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityRepository
            .SignInExternalAsync(new SignInExternal
            {
                ExternalProvider =
                {
                    LoginProvider = logInExternalDirect.ExternalLogInData.ExternalToken.Name,
                    ProviderKey = logInExternalDirect.ExternalLogInData.Id
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

        var refreshTokenExpiry = logInExternalDirect.IsRefreshable
            ? await this.IdentityRepository
                .CreateRefreshToken(identityUser, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, logInExternalDirect.AppId)
            : null;

        if (refreshTokenExpiry != null)
        {
            accessToken.RefreshToken = new RefreshToken
            {
                Token = refreshTokenExpiry.Value,
                ExpireAt = refreshTokenExpiry.ExpireAt
            };
        }

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
            .GenerateJwtTokenByRefreshAsync(new GenerateJwtToken
            {
                Id = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            }, logInRefresh, cancellationToken);

        if (accessToken == null)
        {
            throw new UnauthorizedException();
        }

        var refreshTokenExpiry = await this.IdentityRepository
            .CreateRefreshToken(identityUser, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, appId);

        if (refreshTokenExpiry != null)
        {
            accessToken.RefreshToken = new RefreshToken
            {
                Token = refreshTokenExpiry.Value,
                ExpireAt = refreshTokenExpiry.ExpireAt
            };
        }

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