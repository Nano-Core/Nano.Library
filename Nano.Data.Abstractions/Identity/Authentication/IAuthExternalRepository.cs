using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

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
    Task<ExternalLogInData> AuthenticateAsync<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logInExternalRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExternalLoginTokenData> AuthenticateRefreshAsync(LogInExternalRefresh logInExternalRefresh, CancellationToken cancellationToken = default);
}