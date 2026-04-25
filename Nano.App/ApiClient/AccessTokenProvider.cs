using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Abstractions;
using Nano.App.ApiClient.Exceptions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public sealed class AccessTokenProvider : IAccessTokenProvider
{
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private AccessToken? accessToken;

    /// <inheritdoc />
    public async Task<AccessToken?> GetRootAccessTokenAsync(Func<CancellationToken, Task<AccessToken?>> factory, CancellationToken cancellationToken)
    {
        if (this.accessToken != null)
        {
            return this.accessToken;
        }

        await this.semaphore
            .WaitAsync(cancellationToken);

        try
        {
            return this.accessToken ??= await factory(cancellationToken);
        }
        catch (ApiClientException)
        {
            return null;
        }
        finally
        {
            this.semaphore
                .Release();
        }
    }
}