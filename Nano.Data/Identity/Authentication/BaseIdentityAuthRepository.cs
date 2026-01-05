using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public abstract class BaseIdentityAuthRepository<TIdentity> : IIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IIdentityRepository<TIdentity> identityRepository;
    private readonly IAuthJwtRepository authJwtRepository;
    private readonly IAuthExternalRepository authExternalRepository;

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="identityRepository"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    protected BaseIdentityAuthRepository(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository = null)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<AuthenticationScheme>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        return this.identityRepository
            .GetExternalProviderSchemesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        if (logIn == null) 
            throw new ArgumentNullException(nameof(logIn));
        
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
                .CreateRefreshToken(identityUser, logIn.AppId)
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
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        if (logInExternalDirect == null) 
            throw new ArgumentNullException(nameof(logInExternalDirect));
        
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
                Claims = claims,
                ExternalToken = logInExternalDirect.ExternalLogInData.ExternalToken
            });

        if (logInExternalDirect.IsRefreshable)
        {
            var refreshTokenExpiry = await this.identityRepository
                .CreateRefreshToken(identityUser, logInExternalDirect.AppId);

            accessToken.RefreshToken = new RefreshToken
            {
                Token = refreshTokenExpiry.Value,
                ExpireAt = refreshTokenExpiry.ExpireAt
            };
        }

        return accessToken; 
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
            throw new ArgumentNullException(nameof(logInExternalTransient));

        var externalLoginData = await this.authExternalRepository
            .AuthenticateAsync(logInExternalTransient.Provider, cancellationToken);

        if (externalLoginData == null)
        {
            throw new UnauthorizedException();
        }

        return await this.LogInExternalAsync(new LogInExternalDirect
        {
            AppId = logInExternalTransient.AppId,
            IsRefreshable = logInExternalTransient.IsRefreshable,
            IsRememberMe = logInExternalTransient.IsRememberMe,
            ExternalLogInData = externalLoginData,
            TransientRoles = logInExternalTransient.TransientRoles,
            TransientClaims = logInExternalTransient.TransientClaims
        }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        if (logInRefresh == null) 
            throw new ArgumentNullException(nameof(logInRefresh));
        
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

        ExternalLoginTokenData externalLoginTokenData = null;
        // BUG: Refresh external
        //if (this.authExternalRepository != null)
        //{
        //    var externalProviderName = claims
        //        .Where(x => x.Type == ClaimTypesExtended.ExternalProviderName)
        //        .Select(x => x.Value)
        //        .FirstOrDefault();

        //    var externalProviderRefreshToken = claims
        //        .Where(x => x.Type == ClaimTypesExtended.ExternalProviderRefreshToken)
        //        .Select(x => x.Value)
        //        .FirstOrDefault();

        //    externalLoginTokenData = await this.authExternalRepository
        //        .LogInExternalRefreshAsync(new LogInExternalRefresh
        //        {
        //            ProviderName = externalProviderName,
        //            RefreshToken = externalProviderRefreshToken
        //        }, cancellationToken);
        //}

        this.authJwtRepository
            .ValidateTokenForRefresh(logInRefresh.Token);

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
            .CreateRefreshToken(identityUser, appId);

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
    public virtual Task LogOutAsync(CancellationToken cancellationToken = default)
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