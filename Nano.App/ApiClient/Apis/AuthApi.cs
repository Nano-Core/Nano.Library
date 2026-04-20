using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Apis;

/// <summary>
/// Client for interacting with authentication endpoints (route: <c>auth/*</c>).
/// Provides login flows, token management, and external authentication support.
/// </summary>
public sealed class AuthApi(ApiClient api)
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Executes <c>auth/external-schemes</c> to retrieve available external login providers.
    /// </summary>
    /// <param name="request">The request configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of available external login providers.</returns>
    public async Task<IEnumerable<ExternalLoginProvider>> GetExternalSchemesAsync(GetExternalSchemesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<GetExternalSchemesRequest, IEnumerable<ExternalLoginProvider>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>auth/login</c> to authenticate a user and obtain an access token.
    /// Sets the authorization header on success.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if authentication fails.</exception>
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
    /// Executes <c>auth/login/root</c> to authenticate using root credentials.
    /// Sets the authorization header on success.
    /// </summary>
    /// <param name="request">The root login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if authentication fails.</exception>
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
    /// Executes <c>auth/login/apikey</c> to authenticate using an API key.
    /// Sets the authorization header on success.
    /// </summary>
    /// <param name="request">The API key login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if authentication fails.</exception>
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
    /// Executes <c>auth/login/external</c> to authenticate via an external provider.
    /// Sets the authorization header on success.
    /// </summary>
    /// <typeparam name="TRequest">The external login request type.</typeparam>
    /// <typeparam name="TFlow">The authentication flow type.</typeparam>
    /// <param name="request">The external login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if authentication fails.</exception>
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
    /// Executes <c>auth/login/external/transient</c> to authenticate via an external provider using a transient flow.
    /// Sets the authorization header on success.
    /// </summary>
    /// <typeparam name="TRequest">The external login request type.</typeparam>
    /// <typeparam name="TFlow">The authentication flow type.</typeparam>
    /// <param name="request">The external login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The issued access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if authentication fails.</exception>
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
    /// Executes <c>auth/login/refresh</c> to refresh an access token.
    /// Sets the authorization header on success.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refreshed access token.</returns>
    /// <exception cref="UnauthorizedException">Thrown if refresh fails.</exception>
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
    /// Executes <c>auth/logout</c> to invalidate the current session or token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
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