using System.Threading;
using System.Threading.Tasks;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.Common.Identity.Authentication.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthExternalGoogleRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLogInData> Authenticate(ExternalLoginProviderGoogle provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="externalRefreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(string name, string externalRefreshToken = null, CancellationToken cancellationToken = default);
}