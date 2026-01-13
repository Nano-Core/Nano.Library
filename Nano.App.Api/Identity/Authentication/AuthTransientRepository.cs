using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Config;
using Nano.App.Api.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.App.Api.Identity.Authentication;

/// <inheritdoc />
public class AuthTransientRepository : IAuthTransientRepository
{
    private readonly ExternalLoginOptions options;
    private readonly IAuthJwtRepository authJwtRepository;
    private readonly IAuthExternalRepository authExternalRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    public AuthTransientRepository(ExternalLoginOptions options, IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var schemes = new List<ExternalLoginProvider>();

        if (this.options.Facebook != null)
        {
            const string NAME = nameof(this.options.Facebook);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.options.Google != null)
        {
            const string NAME = nameof(this.options.Google);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        if (this.options.Microsoft != null)
        {
            const string NAME = nameof(this.options.Microsoft);

            schemes
                .Add(new ExternalLoginProvider
                {
                    Name = NAME,
                    DisplayName = NAME
                });
        }

        return schemes;
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync(LogInExternalDirect externalLogInData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(externalLogInData);

        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        await Task.CompletedTask;

        var claims = externalLogInData.TransientClaims
            .Select(x => new Claim(x.Key, x.Value));

        var roleClaims = externalLogInData.TransientRoles
            .Select(x => new Claim(ClaimTypes.Role, x));

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                AppId = externalLogInData.AppId,
                UserId = externalLogInData.ExternalLogInData.Id,
                UserName = externalLogInData.ExternalLogInData.Name,
                UserEmail = externalLogInData.ExternalLogInData.Email,
                ExternalToken = externalLogInData.ExternalLogInData.ExternalToken,
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

        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalLoginData = await this.authExternalRepository
            .AuthenticateAsync(logInExternal.Provider, cancellationToken);

        if (externalLoginData == null)
        {
            throw new UnauthorizedException();
        }

        return await this.LogInExternalAsync(new LogInExternalDirect
        {
            AppId = logInExternal.AppId,
            IsRefreshable = logInExternal.IsRefreshable,
            IsRememberMe = logInExternal.IsRememberMe,
            ExternalLogInData = externalLoginData,
            TransientRoles = logInExternal.TransientRoles,
            TransientClaims = logInExternal.TransientClaims
        }, cancellationToken);
    }
}