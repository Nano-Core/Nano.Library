using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Extensions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Helpers;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthTransientRepository(IAuthJwtRepository authJwtRepository, IAuthExternalRepositoryAggregator authExternalRepository)
    : IAuthTransientRepository
{
    /// <summary>
    /// Get or set the authentication JWT repository.
    /// </summary>
    protected readonly IAuthJwtRepository authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));

    /// <summary>
    /// Get or set the authentication external repository.
    /// </summary>
    protected readonly IAuthExternalRepositoryAggregator authExternalRepository = authExternalRepository ?? throw new ArgumentNullException(nameof(authExternalRepository));

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternal logInExternal, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternal);

        await Task.CompletedTask;

        var claims = logInExternal.TransientClaims
            .Select(x => new Claim(x.Key, x.Value));

        var roleClaims = logInExternal.TransientRoles
            .Select(x => new Claim(ClaimTypes.Role, x));

        var manifestClaim = TransientClaimsManifest
            .Build(logInExternal.TransientRoles, logInExternal.TransientClaims);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternal.AppId,
                UserId = logInExternal.ExternalAuthenticationData.Id,
                UserName = logInExternal.ExternalAuthenticationData.Name,
                UserEmail = logInExternal.ExternalAuthenticationData.EmailAddress,
                ExternalToken = logInExternal.ExternalAuthenticationData.ExternalToken,
                Claims = claims
                    .Union(roleClaims)
                    .Append(manifestClaim)
            });

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TFlow>(string providerName, LogInExternal<TFlow> logInExternal, CancellationToken cancellationToken = default)
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(logInExternal);

        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var authenticationData = await this.authExternalRepository
            .AuthenticateAsync(providerName, logInExternal.Flow, cancellationToken);

        if (!logInExternal.IsRefreshable)
        {
            authenticationData.ExternalToken.RefreshToken = null;
        }

        var claims = logInExternal.TransientClaims
            .Merge(authenticationData.TransientClaims);

        return await this.LogInExternalAsync(new LogInExternal
        {
            AppId = logInExternal.AppId,
            IsRefreshable = logInExternal.IsRefreshable,
            ExternalAuthenticationData = authenticationData,
            TransientRoles = logInExternal.TransientRoles,
            TransientClaims = claims
        }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalRefreshAsync(string providerName, string jwtToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(providerName);
        ArgumentNullException.ThrowIfNull(jwtToken);

        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        this.authJwtRepository
            .ValidateTokenForRefresh(jwtToken);

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = jwtSecurityTokenHandler
            .ReadJwtToken(jwtToken);

        var tokenProviderName = jwtSecurityToken.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderName)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderRefreshToken = jwtSecurityToken.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderRefreshToken)
            .Select(x => x.Value)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(tokenProviderName) || string.IsNullOrEmpty(externalProviderRefreshToken))
        {
            throw new UnauthorizedException("The token does not carry a refreshable external login.");
        }

        if (!string.Equals(tokenProviderName, providerName, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException($"The token was issued for provider '{tokenProviderName}', not '{providerName}'.");
        }

        var externalAuthenticationToken = await this.authExternalRepository
            .AuthenticateRefreshAsync(providerName, externalProviderRefreshToken, cancellationToken);

        var appId = jwtSecurityTokenHandler
            .GetJwtAppId(jwtToken) ?? IdentityDefaults.DEFAULT_APP_ID;

        var userId = jwtSecurityTokenHandler
            .GetJwtUserId(jwtToken);

        var userName = jwtSecurityTokenHandler
            .GetJwtUserName(jwtToken);

        var email = jwtSecurityTokenHandler
            .GetJwtUserEmail(jwtToken);

        var (transientRoles, transientClaims) = TransientClaimsManifest
            .Parse(jwtSecurityToken.Claims);

        var transientClaimsManifest = TransientClaimsManifest.Build(transientRoles, transientClaims);

        var claims = transientClaims
            .Select(x => new Claim(x.Key, x.Value))
            .Union(transientRoles.Select(x => new Claim(ClaimTypes.Role, x)))
            .Append(transientClaimsManifest);

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = appId,
                UserId = userId,
                UserName = userName,
                UserEmail = email,
                ExternalToken = externalAuthenticationToken,
                Claims = claims
            });

        return accessToken;
    }
}