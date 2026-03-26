using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Extensions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
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
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalDirect);

        await Task.CompletedTask;

        var claims = logInExternalDirect.TransientClaims
            .Select(x => new Claim(x.Key, x.Value));

        var roleClaims = logInExternalDirect.TransientRoles
            .Select(x => new Claim(ClaimTypes.Role, x));

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = logInExternalDirect.AppId,
                UserId = logInExternalDirect.ExternalAuthenticationData.Id,
                UserName = logInExternalDirect.ExternalAuthenticationData.Name,
                UserEmail = logInExternalDirect.ExternalAuthenticationData.EmailAddress,
                ExternalToken = logInExternalDirect.ExternalAuthenticationData.ExternalToken,
                Claims = claims
                    .Union(roleClaims)
            });

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider, TFlow>(BaseLogInExternal<TProvider, TFlow> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseExternalProvider, new()
        where TFlow : BaseAuthFlow, new()
    {
        ArgumentNullException.ThrowIfNull(logInExternal);

        if (authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(authExternalRepository));
        }

        var authenticationData = await authExternalRepository
            .AuthenticateAsync(logInExternal.Provider, logInExternal.Flow, cancellationToken);

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
}