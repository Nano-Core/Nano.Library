using System.Threading;
using System.Threading.Tasks;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.Common.Identity.Authentication.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAuthExternalRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLogInData> Authenticate<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="externalRefreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(string name = null, string externalRefreshToken = null, CancellationToken cancellationToken = default);
}