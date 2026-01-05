using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Web.Identity.Authentication;

/// <summary>
/// 
/// </summary>
public interface IAuthTransientRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="externalLogInData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalAsync(LogInExternalDirect externalLogInData, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> LogInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new();
}