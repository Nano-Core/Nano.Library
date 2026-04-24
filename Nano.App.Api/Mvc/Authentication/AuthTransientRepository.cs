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
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternal logInExternal, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternal);

        await Task.CompletedTask;

        var claims = logInExternal.TransientClaims
            .Select(x => new Claim(x.Key, x.Value));

        var roleClaims = logInExternal.TransientRoles
            .Select(x => new Claim(ClaimTypes.Role, x));

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
}