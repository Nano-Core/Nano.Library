using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.Web.Identity.Models;

namespace Nano.App.Web.Identity.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IIdentityJwtRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateJwtToken"></param>
    /// <returns></returns>
    AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generateJwtToken"></param>
    /// <param name="logInRefresh"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken> GenerateJwtTokenByRefreshAsync(GenerateJwtToken generateJwtToken, LogInRefresh logInRefresh, CancellationToken cancellationToken = default);
}