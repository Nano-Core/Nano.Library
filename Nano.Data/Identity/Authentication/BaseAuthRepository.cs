using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.Data.Identity.Authentication;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseAuthRepository<TIdentity> : IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IAuthJwtRepository authJwtRepository;
    private readonly IAuthExternalRepository authExternalRepository;
    private readonly IIdentityRepository<TIdentity> identityRepository;

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="identityRepository"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    protected BaseAuthRepository(IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository, IIdentityRepository<TIdentity> identityRepository)
    {
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
        this.authExternalRepository = authExternalRepository ?? throw new ArgumentNullException(nameof(authExternalRepository));
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        return this.identityRepository
            .GetExternalProviderSchemesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInAsync(LogIn logIn, int refreshExpirationInHours, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.identityRepository
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

        var refreshTokenExpiry = logIn.IsRefreshable
            ? await this.identityRepository
                .CreateRefreshToken(identityUser, refreshExpirationInHours, logIn.AppId)
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
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, int refreshExpirationInHours, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternal == null)
            throw new ArgumentNullException(nameof(logInExternal));

        var externalLoginData = await this.authExternalRepository
            .Authenticate(logInExternal.Provider, cancellationToken);

        var identityUser = await this.identityRepository
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

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, logInExternal.TransientRoles, logInExternal.TransientClaims, cancellationToken);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternal.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        var refreshTokenExpiry = logInExternal.IsRefreshable
            ? await this.identityRepository
                .CreateRefreshToken(identityUser, refreshExpirationInHours, logInExternal.AppId)
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
    public virtual async Task<AccessToken> LogInExternalDirectAsync(LogInExternalDirect logInExternalDirect, int refreshExpirationInHours, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.identityRepository
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

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, logInExternalDirect.TransientRoles, logInExternalDirect.TransientClaims, cancellationToken);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternalDirect.AppId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            });

        var refreshTokenExpiry = logInExternalDirect.IsRefreshable
            ? await this.identityRepository
                .CreateRefreshToken(identityUser, refreshExpirationInHours, logInExternalDirect.AppId)
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
    public virtual async Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, int refreshExpirationInHours, CancellationToken cancellationToken = default)
    {
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var userIdString = jwtSecurityTokenHandler
            .GetJwtUserId(logInRefresh.Token);

        var userId = ConvertToIdentity(userIdString);

        var identityUser = await this.identityRepository
            .GetIdentityUserAsync(userId, cancellationToken);

        var appId = jwtSecurityTokenHandler
            .GetJwtAppId(logInRefresh.Token);

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

        var externalLoginTokenData = await this.authExternalRepository
            .AuthenticateRefresh(externalProviderName, externalProviderRefreshToken, cancellationToken);

        this.authJwtRepository
            .ValidateRefreshToken(logInRefresh.RefreshToken);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = appId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                ExternalToken = externalLoginTokenData,
                Claims = claims
            });

        if (accessToken == null)
        {
            throw new UnauthorizedException();
        }

        var refreshTokenExpiry = await this.identityRepository
            .CreateRefreshToken(identityUser, refreshExpirationInHours, appId);

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
        return this.identityRepository
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