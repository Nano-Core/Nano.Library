using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.App.Api.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalRepository : IAuthExternalRepository
{
    private readonly IAuthExternalFacebookRepository facebookRepository;
    private readonly IAuthExternalGoogleRepository googleRepository;
    private readonly IAuthExternalMicrosoftRepository microsoftRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="facebookRepository"></param>
    /// <param name="googleRepository"></param>
    /// <param name="microsoftRepository"></param>
    public AuthExternalRepository(IAuthExternalFacebookRepository facebookRepository = null, IAuthExternalGoogleRepository googleRepository = null, IAuthExternalMicrosoftRepository microsoftRepository = null)
    {
        this.facebookRepository = facebookRepository;
        this.googleRepository = googleRepository;
        this.microsoftRepository = microsoftRepository;
    }

    /// <inheritdoc />
    public virtual Task<ExternalLogInData> AuthenticateAsync<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (provider is ExternalLoginProviderFacebook facebookProvider)
        {
            if (this.facebookRepository == null)
            {
                throw new NullReferenceException(nameof(this.facebookRepository));
            }

            return this.facebookRepository
                .Authenticate(facebookProvider, cancellationToken);
        }

        if (provider is ExternalLoginProviderGoogle googleProvider)
        {
            if (this.googleRepository == null)
            {
                throw new NullReferenceException(nameof(this.googleRepository));
            }

            return this.googleRepository
                .Authenticate(googleProvider, cancellationToken);
        }

        if (provider is ExternalLoginProviderMicrosoft microsoftProvider)
        {
            if (this.microsoftRepository == null)
            {
                throw new NullReferenceException(nameof(this.microsoftRepository));
            }

            return this.microsoftRepository
                .Authenticate(microsoftProvider, cancellationToken);
        }

        throw new UnauthorizedException();
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefreshAsync(LogInExternalRefresh logInExternalRefresh, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalRefresh);

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