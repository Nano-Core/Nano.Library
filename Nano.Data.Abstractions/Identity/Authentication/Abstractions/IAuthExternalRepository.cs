using Nano.Data.Abstractions.Identity.Authentication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Identity.Authentication.Abstractions;

// TODO: SSO implementation Test and improvements (Facebook, Apple, Google, Microsoft) (remember inject HttpClient)

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