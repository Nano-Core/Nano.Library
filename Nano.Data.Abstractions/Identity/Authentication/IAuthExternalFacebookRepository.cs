using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// 
/// </summary>
public interface IAuthExternalFacebookRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLogInData> Authenticate(ExternalLoginProviderFacebook provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshFacebook logInExternalRefresh, CancellationToken cancellationToken = default);
}