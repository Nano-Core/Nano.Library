using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Abstractions;

/// <summary>
/// Provides a mechanism for retrieving an access token, typically for root-level or system-level authentication scenarios.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Retrieves a root access token using the provided factory method.
    /// </summary>
    /// <param name="factory">A delegate responsible for creating the access token asynchronously.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AccessToken"/> if successful; otherwise, <c>null</c>.</returns>
    Task<AccessToken?> GetRootAccessTokenAsync(Func<CancellationToken, Task<AccessToken?>> factory, CancellationToken cancellationToken);
}