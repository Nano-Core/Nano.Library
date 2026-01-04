using Nano.App.Web.Config;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Web.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalRepository : IAuthExternalRepository
{
    private readonly ExternalLoginOptions options;
    private readonly IAuthJwtRepository authJwtRepository;
    private readonly IAuthExternalFacebookRepository facebookRepository;
    private readonly IAuthExternalGoogleRepository googleRepository;
    private readonly IAuthExternalMicrosoftRepository microsoftRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="facebookRepository"></param>
    /// <param name="googleRepository"></param>
    /// <param name="microsoftRepository"></param>
    public AuthExternalRepository(ExternalLoginOptions options, IAuthJwtRepository authJwtRepository, IAuthExternalFacebookRepository facebookRepository = null, IAuthExternalGoogleRepository googleRepository = null, IAuthExternalMicrosoftRepository microsoftRepository = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.authJwtRepository = authJwtRepository ?? throw new ArgumentNullException(nameof(authJwtRepository));
        this.facebookRepository = facebookRepository;
        this.googleRepository = googleRepository;
        this.microsoftRepository = microsoftRepository;
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
    public virtual Task<ExternalLogInData> GetExternalLogInData<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        if (this.facebookRepository != null && provider is ExternalLoginProviderFacebook facebookProvider)
        {
            return this.facebookRepository
                .Authenticate(facebookProvider, cancellationToken);
        }

        if (this.googleRepository != null && provider is ExternalLoginProviderGoogle googleProvider)
        {
            return this.googleRepository
                .Authenticate(googleProvider, cancellationToken);
        }

        if (this.microsoftRepository != null && provider is ExternalLoginProviderMicrosoft microsoftProvider)
        {
            return this.microsoftRepository
                .Authenticate(microsoftProvider, cancellationToken);
        }

        throw new UnauthorizedException();
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
            throw new ArgumentNullException(nameof(logInExternalTransient));

        var externalLoginData = await this.GetExternalLogInData(logInExternalTransient.Provider, cancellationToken);

        if (externalLoginData == null)
        {
            throw new UnauthorizedException();
        }

        return await this.LogInExternalAsync(externalLoginData, logInExternalTransient.TransientRoles, logInExternalTransient.TransientClaims, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<AccessToken> LogInExternalAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        await Task.CompletedTask;

        var claims = transientClaims?
            .Select(x => new Claim(x.Key, x.Value)) ?? new List<Claim>();

        var roleClaims = transientRoles?
            .Select(x => new Claim(ClaimTypes.Role, x)) ?? new List<Claim>();

        var accessToken = this.authJwtRepository
            .GenerateJwtToken(new GenerateJwtToken
            {
                UserId = externalLogInData.Id,
                UserName = externalLogInData.Name,
                UserEmail = externalLogInData.Email,
                ExternalToken = externalLogInData.ExternalToken,
                Claims = claims
                    .Union(roleClaims)
            });

        return accessToken;
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> LogInExternalRefreshAsync(LogInExternalRefresh logInExternalRefresh, CancellationToken cancellationToken = default)
    {
        if (logInExternalRefresh == null) 
            throw new ArgumentNullException(nameof(logInExternalRefresh));
        
        await Task.CompletedTask;

        if (string.IsNullOrEmpty(logInExternalRefresh.ProviderName) || string.IsNullOrEmpty(logInExternalRefresh.RefreshToken))
        {
            throw new UnauthorizedException();
        }

        if (this.facebookRepository != null && logInExternalRefresh.ProviderName == "Facebook")
        {
            return await this.facebookRepository
                .AuthenticateRefresh(new LogInExternalRefreshFacebook
                {
                    RefreshToken = logInExternalRefresh.RefreshToken
                }, cancellationToken);
        }

        if (this.googleRepository != null && logInExternalRefresh.ProviderName == "Google")
        {
            return await this.googleRepository
                .AuthenticateRefresh(new LogInExternalRefreshGoogle
                {
                    RefreshToken = logInExternalRefresh.RefreshToken
                }, cancellationToken);
        }

        if (this.microsoftRepository != null && logInExternalRefresh.ProviderName == "Microsoft")
        {
            return await this.microsoftRepository
                .AuthenticateRefresh(new LogInExternalRefreshMicrosoft
                {
                    RefreshToken = logInExternalRefresh.RefreshToken
                }, cancellationToken);
        }

        throw new UnauthorizedException();
    }
}