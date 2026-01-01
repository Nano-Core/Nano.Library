using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Web.Identity.Authentication;

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
    public AuthExternalRepository(IAuthExternalFacebookRepository facebookRepository, IAuthExternalGoogleRepository googleRepository, IAuthExternalMicrosoftRepository microsoftRepository)
    {
        this.facebookRepository = facebookRepository;
        this.googleRepository = googleRepository;
        this.microsoftRepository = microsoftRepository;
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        return provider switch
        {
            ExternalLoginProviderFacebook facebookProvider => await this.facebookRepository.Authenticate(facebookProvider, cancellationToken),
            ExternalLoginProviderGoogle googleProvider => await this.googleRepository.Authenticate(googleProvider, cancellationToken),
            ExternalLoginProviderMicrosoft microsoftProvider => await this.microsoftRepository.Authenticate(microsoftProvider, cancellationToken),
            _ => throw new NotSupportedException(nameof(provider))
        };
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefresh(string name = null, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new ExternalLoginTokenData();
        }

        if (string.IsNullOrEmpty(externalRefreshToken))
        {
            return new ExternalLoginTokenData();
        }

        return name switch
        {
            "Facebook" => await this.facebookRepository.AuthenticateRefresh(name, externalRefreshToken, cancellationToken),
            "Google" => await this.googleRepository.AuthenticateRefresh(name, externalRefreshToken, cancellationToken),
            "Microsoft" => await this.microsoftRepository.AuthenticateRefresh(name, externalRefreshToken, cancellationToken),
            _ => throw new NotSupportedException($"The external provider: {name} is not supported.")
        };
    }
}