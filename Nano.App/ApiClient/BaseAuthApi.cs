using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
public abstract class BaseAuthApi<TIdentity> : BaseApi<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseAuthApi(IOptionsMonitor<ApiOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }

    /// <summary>
    /// Log-In Async.
    /// </summary>
    /// <param name="request">The <see cref="LogInRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> LogInAsync(LogInRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await this.InvokeAsync<LogInRequest, AccessToken>(request, cancellationToken);

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
    public virtual async Task<AccessToken> LogInRootAsync(LogInRootRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await this.InvokeAsync<LogInRootRequest, AccessToken>(request, cancellationToken);

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
    /// <param name="request">The <see cref="BaseLogInExternalRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> LogInExternalAsync<TLogin>(TLogin request, CancellationToken cancellationToken = default)
        where TLogin : BaseLogInExternalRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await this.InvokeAsync<TLogin, AccessToken>(request, cancellationToken);

        if (response == null)
        {
            return null;
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
    public virtual async Task<AccessToken> LogInRefreshAsync(LogInRefreshRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var response = await this.InvokeAsync<LogInRefreshRequest, AccessToken>(request, cancellationToken);

        this.SetAuthorizationHeader(response.Token);

        if (response == null)
        {
            throw new UnauthorizedException();
        }

        return response;
    }

    /// <summary>
    /// Log-Out Async.
    /// </summary>
    /// <param name="request">The <see cref="LogOutRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void..</returns>
    public virtual Task LogOutAsync(LogOutRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get External Login Data Async.
    /// </summary>
    /// <param name="request">The <see cref="BaseGetExternalLoginDataRequest{TProvider}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<ExternalLogInData> GetExternalLoginDataAsync<TProvider>(BaseGetExternalLoginDataRequest<TProvider> request, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<BaseGetExternalLoginDataRequest<TProvider>, ExternalLogInData>(request, cancellationToken);
    }

    /// <summary>
    /// GetAsync External Schemes Async.
    /// </summary>
    /// <param name="request">The <see cref="GetExternalSchemesRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>A collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    public virtual Task<IEnumerable<ExternalLoginProvider>> GetExternalSchemesAsync(GetExternalSchemesRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<GetExternalSchemesRequest, IEnumerable<ExternalLoginProvider>>(request, cancellationToken);
    }


    private void SetAuthorizationHeader(string token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        this.httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization] = $"Bearer {token}";
    }
}