using Microsoft.AspNetCore.Authentication;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthTransientRepository(IAuthenticationSchemeProvider authenticationSchemeProvider, IAuthJwtRepository authJwtRepository, IAuthExternalRepositoryAggregator authExternalRepository)
    : IAuthTransientRepository
{
    /// <summary>
    /// Get or set the authentication JWT repository.
    /// </summary>
    protected readonly IAuthJwtRepository authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));

    /// <summary>
    /// Get or set the authentication scheme provideder.
    /// </summary>
    protected readonly IAuthenticationSchemeProvider authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));

    /// <summary>
    /// Get or set the authentication external repository.
    /// </summary>
    protected readonly IAuthExternalRepositoryAggregator authExternalRepository = authExternalRepository ?? throw new ArgumentNullException(nameof(authExternalRepository));

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await this.authenticationSchemeProvider
            .GetAllSchemesAsync();

        return schemes
            .Where(s => !string.IsNullOrEmpty(s.DisplayName))
            .Select(x => new ExternalLoginProvider
            {
                Name = x.Name,
                DisplayName = x.DisplayName
            });
    }

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
                UserId = logInExternalDirect.ExternalLogInData.Id,
                UserName = logInExternalDirect.ExternalLogInData.Name,
                UserEmail = logInExternalDirect.ExternalLogInData.Email,
                ExternalToken = logInExternalDirect.ExternalLogInData.ExternalToken,
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

        var externalLoginData = await authExternalRepository
            .AuthenticateAsync(logInExternal.Provider, logInExternal.Flow, cancellationToken);

        if (externalLoginData == null)
        {
            throw new UnauthorizedException();
        }

        return await this.LogInExternalAsync(new LogInExternalDirect
        {
            AppId = logInExternal.AppId,
            IsRefreshable = logInExternal.IsRefreshable,
            ExternalLogInData = externalLoginData,
            TransientRoles = logInExternal.TransientRoles,
            TransientClaims = logInExternal.TransientClaims
        }, cancellationToken);
    }
}