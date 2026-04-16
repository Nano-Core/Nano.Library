using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
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
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.App.ApiClient;

// BUG: CHAT-GPT: talk about the injection of HttpClient in constructor, and if it's safe.
// BUG: CHAT-GPT: and triple slash

/// <summary>
/// Base Api (abstract).
/// </summary>
public class ApiClient
{
    private volatile AccessToken? accessToken; // BUG: Volatile??? Refactor where it's used and it should be static and concurrent.

    private readonly ApiClientOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// The httpcontext accessor.
    /// Use to acecss headers and jwt token to pass through the api-client request.
    /// </summary>
    protected internal readonly IHttpContextAccessor? httpContextAccessor;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="ApiClientOptions"/>.</param>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
    protected internal ApiClient(ApiClientOptions options, HttpClient httpClient, IHttpContextAccessor? httpContextAccessor = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Invokes the request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    protected internal virtual async Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
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
    protected internal virtual async Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
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
    protected internal virtual async Task<TResponse?> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
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
    private async Task<HttpRequestMessage> GetHttpRequestMessage<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var method = request
            .GetMethod();

        var uri = this.GetUri(request);
        var jwtToken = await this.AuthenticateAsync(request, cancellationToken);

        StringValues requestIdHeader = default;

        var httpContext = this.httpContextAccessor?.HttpContext;

        httpContext?.Request.Headers
            .TryGetValue(NanoHeaderNames.REQUEST_ID, out requestIdHeader);

        var httpRequestMessage = new HttpRequestMessage(method, uri);

        var headersToForward = new[]
        {
            NanoHeaderNames.X_API_KEY,
            NanoHeaderNames.X_FORWARDED_PROTO,
            NanoHeaderNames.X_FORWARDED_HOST,
            NanoHeaderNames.X_FORWARDED_PORT,
            NanoHeaderNames.X_FORWARDED_FOR,
            NanoHeaderNames.X_FORWARDED_PREFIX,
            NanoHeaderNames.REQUEST_ID,
            HeaderNames.AcceptLanguage,
            RequestTimeZoneHeaderProvider.Headerkey
        };

        if (httpContext != null)
        {
            foreach (var header in headersToForward)
            {
                if (httpContext.Request.Headers.TryGetValue(header, out var value))
                {
                    httpRequestMessage.Headers
                        .TryAddWithoutValidation(header, value.ToArray());
                }
            }
        }

        if (jwtToken != null)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        await httpRequestMessage
            .AddHttpHeaders(request, jwtToken, requestIdHeader, cancellationToken);

        await httpRequestMessage
            .AddHttpBody(request, cancellationToken);

        await httpRequestMessage
            .AddHttpForm(request, cancellationToken);

        return httpRequestMessage;
    }
    private async Task<string?> AuthenticateAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        if (request.GetType() == typeof(LogInRequest))
        {
            return null;
        }

        if (request.JwtTokenOverride != null)
        {
            return request.JwtTokenOverride;
        }

        if (this.options.LogIn is not null)
        {
            var logInRootRequest = new LogInRootRequest
            {
                LogInRoot = new LogInRoot
                {
                    Username = this.options.LogIn.Username,
                    Password = this.options.LogIn.Password
                }
            };

            this.accessToken = await this.InvokeAsync<LogInRootRequest, AccessToken>(logInRootRequest, cancellationToken);

            return this.accessToken?.Token;
        }

        // BUG: make this second choice before Root Login
        var jwtToken = this.httpContextAccessor?.HttpContext?
            .GetJwtToken();

        return jwtToken ?? this.accessToken?.Token;
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

/// <summary>
/// 
/// </summary>
public abstract class BaseApi(ApiClient apiClient) : BaseApi<Guid>(apiClient);

/// <summary>
/// 
/// </summary>
public abstract class BaseApi<TIdentity>(ApiClient apiClient)
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    public AuthApi Auth { get; } = new(apiClient);

    /// <summary>
    /// 
    /// </summary>
    public AuditApi Audit { get; } = new(apiClient);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <param name="apiClient"></param>
public abstract class BaseIdentityApi<TUser>(ApiClient apiClient) : BaseIdentityApi<TUser, Guid>(apiClient)
    where TUser : class, IEntityUser<Guid>;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseIdentityApi<TUser, TIdentity>(ApiClient apiClient) : BaseApi<TIdentity>(apiClient)
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    public IdentityApi<TUser, TIdentity> Identity { get; } = new(apiClient);
}
