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
public class AuthTransientRepository(IAuthenticationSchemeProvider schemeProvider, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
    : IAuthTransientRepository
{
    private readonly IAuthJwtRepository authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
    private readonly IAuthenticationSchemeProvider schemeProvider = schemeProvider ?? throw new ArgumentNullException(nameof(schemeProvider));

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await this.schemeProvider
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
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        ArgumentNullException.ThrowIfNull(logInExternal);

        if (authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(authExternalRepository));
        }

        var externalLoginData = await authExternalRepository
            .AuthenticateAsync(logInExternal.Provider, cancellationToken);

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