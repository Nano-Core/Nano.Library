using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
public sealed class AuthApi(ApiClient api)
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Get External Schemes Async.
    /// </summary>
    /// <param name="request">The <see cref="GetExternalSchemesRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    public async Task<IEnumerable<ExternalLoginProvider>> GetExternalSchemesAsync(GetExternalSchemesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<GetExternalSchemesRequest, IEnumerable<ExternalLoginProvider>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Log-In Async.
    /// </summary>
    /// <param name="request">The <see cref="LogInRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInAsync(LogInRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<LogInRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-In Root Async.
    /// </summary>
    /// <param name="request">The <see cref="LogInRootRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInRootAsync(LogInRootRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<LogInRootRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-In Api Key Async.
    /// </summary>
    /// <param name="request">The <see cref="LogInRootRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInApiKeyAsync(LogInApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<LogInApiKeyRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-In External Async.
    /// </summary>
    /// <typeparam name="TRequest">The type of external login request.</typeparam>
    /// <typeparam name="TFlow">The type of flow used bty the external login request.</typeparam>
    /// <param name="request">The <see cref="BaseLogInExternalRequest{TFlow}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInExternalAsync<TRequest, TFlow>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseLogInExternalRequest<TFlow>
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<TRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-In External Transient Async.
    /// </summary>
    /// <typeparam name="TRequest">The type of external login request.</typeparam>
    /// <typeparam name="TFlow">The type of flow used bty the external login request.</typeparam>
    /// <param name="request">The <see cref="BaseLogInExternalRequest{TFlow}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInExternalTransientAsync<TRequest, TFlow>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseLogInExternalTransientRequest<TFlow>
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<TRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-In Refresh Async.
    /// </summary>
    /// <param name="request">The <see cref="LogInRefreshRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public async Task<AccessToken> LogInRefreshAsync(LogInRefreshRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<LogInRefreshRequest, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        this.SetAuthorizationHeader(response.Token);

        return response;
    }

    /// <summary>
    /// Log-Out Async.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void..</returns>
    public Task LogOutAsync(CancellationToken cancellationToken = default)
    {
        return this.api
            .InvokeAsync(new LogOutRequest(), cancellationToken);
    }


    private void SetAuthorizationHeader(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        this.api.httpContextAccessor?.HttpContext?.Request.Headers[HeaderNames.Authorization] = $"Bearer {token}";
    }
}