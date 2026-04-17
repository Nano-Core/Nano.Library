using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccessToken?> GetRootAccessTokenAsync(Func<CancellationToken, Task<AccessToken?>> factory, CancellationToken cancellationToken);
}