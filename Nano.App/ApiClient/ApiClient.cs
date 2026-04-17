using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Nano.App.ApiClient.Abstractions;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Extensions;
using Nano.App.ApiClient.Models;
using Nano.App.ApiClient.Requests;
using Nano.App.ApiClient.Requests.Auth;
using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.App.Exceptions;
using Nano.Common.Consts;
using Nano.Common.Serialization;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Extensions;
using Nano.Data.Abstractions.Models.Abstractions;
using Newtonsoft.Json;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.App.ApiClient;

/// <summary>
/// Base Api (abstract).
/// </summary>
public class ApiClient
{
    private static readonly string[] headersToForward =
    [
        NanoHeaderNames.X_API_KEY,
        NanoHeaderNames.X_FORWARDED_PROTO,
        NanoHeaderNames.X_FORWARDED_HOST,
        NanoHeaderNames.X_FORWARDED_PORT,
        NanoHeaderNames.X_FORWARDED_FOR,
        NanoHeaderNames.X_FORWARDED_PREFIX,
        NanoHeaderNames.REQUEST_ID,
        HeaderNames.AcceptLanguage,
        RequestTimeZoneHeaderProvider.Headerkey
    ];

    private readonly ApiClientOptions options;
    private readonly HttpClient httpClient;
    private readonly IAccessTokenProvider accessTokenProvider;
    internal readonly IHttpContextAccessor? httpContextAccessor;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="ApiClientOptions"/>.</param>
    /// <param name="accessTokenProvider"></param>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
    protected internal ApiClient(ApiClientOptions options, HttpClient httpClient, IAccessTokenProvider accessTokenProvider, IHttpContextAccessor? httpContextAccessor = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Invokes the request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    internal virtual async Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    internal virtual async Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller ??= GetInferredController<TResponse>();

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TEntity">The entity.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    internal virtual async Task<TResponse?> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller ??= GetInferredController<TEntity>();

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }


    private Uri GetUri<TRequest>(TRequest request)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var protocol = this.options.UseSsl
            ? "https://"
            : "http://";
        var host = this.options.Host.EndsWith('/')
            ? this.options.Host[..^1]
            : this.options.Host;
        var port = this.options.Port;
        var root = this.options.Root.EndsWith('/')
            ? this.options.Root[..^1]
            : this.options.Root;

        var controller = string.IsNullOrEmpty(request.Controller)
            ? null
            : $"{request.Controller}/";

        var action = request
            .GetAction();

        var routeParameters = request
            .GetRouteParameters();

        var route = action
            .SmartFormat(routeParameters);

        var queryString = request
            .GetQuerystring();

        var uri = $"{protocol}{host}:{port}/{root}/{controller}{route}?{queryString}";

        return new Uri(uri);
    }
    private async Task<AccessToken?> GetAccessToken<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var accessToken2 = request.JwtTokenOverride ?? this.httpContextAccessor?.HttpContext?.GetJwtToken();
        var accessToken = accessToken2 == null ? null : new AccessToken { Token = accessToken2 };

        if (accessToken == null && request is not LogInRootRequest && this.options.LogInRoot is not null)
        {
            accessToken ??= await this.accessTokenProvider
                .GetRootAccessTokenAsync(x => this.InvokeAsync<LogInRootRequest, AccessToken>(new LogInRootRequest
                {
                    LogInRoot = new LogInRoot
                    {
                        Username = this.options.LogInRoot.Username,
                        Password = this.options.LogInRoot.Password
                    }
                }, x), cancellationToken);
        }

        return accessToken;
    }

    private async Task<HttpRequestMessage> GetHttpRequestMessage<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var uri = this.GetUri(request);
        var method = request.GetMethod();
        var httpRequestMessage = new HttpRequestMessage(method, uri);
        var accessToken = await this.GetAccessToken(request, cancellationToken);

        if (accessToken != null)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
        }

        await httpRequestMessage
            .AddHttpHeaders(request, cancellationToken);

        if (this.httpContextAccessor?.HttpContext != null)
        {
            foreach (var header in headersToForward)
            {
                if (!this.httpContextAccessor.HttpContext.Request.Headers.TryGetValue(header, out var value))
                {
                    continue;
                }

                if (httpRequestMessage.Headers.Contains(header))
                {
                    continue;
                }

                httpRequestMessage.Headers
                    .Add(header, value.ToArray());
            }
        }

        await httpRequestMessage
            .AddHttpBody(request, cancellationToken);

        await httpRequestMessage
            .AddHttpForm(request, cancellationToken);

        return httpRequestMessage;
    }

    private static string GetInferredController<TResponse>()
        where TResponse : class
    {
        var type = typeof(TResponse);

        return type.IsGenericType
            ? $"{type.GenericTypeArguments[0].Name}s"
            : $"{type.Name.ToLower()}s";
    }
    private static async Task GetResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        switch ((int)httpResponse.StatusCode)
        {
            case >= 400 and < 600:
            {
                var content = await httpResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                try
                {
                    var serializerSettings = SerializerSettings.GetDefault();
                    var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content, serializerSettings);

                    if (problemDetails == null)
                    {
                        throw new NullReferenceException(nameof(problemDetails));
                    }

                    throw new ProblemDetailsException(problemDetails);
                }
                catch (JsonException)
                {
                    var exceptionMessage = content
                        .RemoveQuotes();

                    throw new Exception(exceptionMessage);
                }
            }

            default:
                httpResponse
                    .EnsureSuccessStatusCode();

                break;
        }
    }
    private static async Task<TResponse?> GetResponseAsync<TResponse>(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        await GetResponseAsync(httpResponse, cancellationToken);

        if (httpResponse.Content.Headers.ContentDisposition != null)
        {
            var stream = await httpResponse.Content
                .ReadAsStreamAsync(cancellationToken);

            if (typeof(TResponse) != typeof(NamedStream))
            {
                return stream as TResponse;
            }

            var name = httpResponse.Content.Headers.ContentDisposition?.FileName;

            if (name == null)
            {
                throw new NullReferenceException(nameof(name));
            }

            var namedStream = new NamedStream
            {
                Name = name,
                Stream = stream
            };

            return namedStream as TResponse;
        }

        var content = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        content = content.Trim();

        try
        {
            return JsonConvert.DeserializeObject<TResponse>(content, SerializerSettings.GetDefault());
        }
        catch (JsonException)
        {
            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)content;
            }

            throw;
        }
    }
}