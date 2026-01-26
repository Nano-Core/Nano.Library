using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Extensions;
using Nano.App.ApiClient.Models;
using Nano.App.ApiClient.Requests;
using Nano.App.ApiClient.Requests.Auth;
using Nano.App.Exceptions;
using Nano.Common.Serialization.Json;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;
using Nano.Data.Abstractions.Identity.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Models.Auth;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models.Abstractions;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.App.ApiClient;

/// <summary>
/// Base Api (abstract).
/// </summary>
public abstract class BaseApi
{
    private volatile AccessToken? accessToken;

    private readonly IOptionsMonitor<ApiClientOptions> apiOptions;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="apiOptions">The <see cref="ApiClientOptions"/>.</param>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
    protected BaseApi(IOptionsMonitor<ApiClientOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Invokes the request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    protected virtual Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        return request switch
        {
            BaseRequestGet requestGet => this.GetAsync(requestGet, cancellationToken),
            BaseRequestPut requestPut => this.PutAsync(requestPut, cancellationToken),
            BaseRequestPost requestPost => this.PostAsync(requestPost, cancellationToken),
            BaseRequestPostForm requestPostForm => this.PostFormAsync(requestPostForm, cancellationToken),
            BaseRequestDelete requestDelete => this.DeleteAsync(requestDelete, cancellationToken),
            _ => throw new NotSupportedException($"Not supported: {nameof(request)}")
        };
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    protected virtual async Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller ??= GetInferredController<TResponse>();

        return request switch
        {
            BaseRequestGet requestGet => await this.GetAsync<BaseRequestGet, TResponse>(requestGet, cancellationToken),
            BaseRequestPut requestPut => await this.PutAsync<BaseRequestPut, TResponse>(requestPut, cancellationToken),
            BaseRequestPost requestPost => await this.PostAsync<BaseRequestPost, TResponse>(requestPost, cancellationToken),
            BaseRequestPostForm requestPostForm => await this.PostFormAsync<BaseRequestPostForm, TResponse>(requestPostForm, cancellationToken),
            BaseRequestDelete requestDelete => await this.DeleteAsync<BaseRequestDelete, TResponse>(requestDelete, cancellationToken),
            _ => throw new NotSupportedException($"Not supported: {nameof(request)}")
        };
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
    protected virtual async Task<TResponse?> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller ??= GetInferredController<TEntity>();

        return request switch
        {
            BaseRequestGet requestGet => await this.GetAsync<BaseRequestGet, TResponse>(requestGet, cancellationToken),
            BaseRequestPut requestPut => await this.PutAsync<BaseRequestPut, TResponse>(requestPut, cancellationToken),
            BaseRequestPost requestPost => await this.PostAsync<BaseRequestPost, TResponse>(requestPost, cancellationToken),
            BaseRequestPostForm requestPostForm => await this.PostFormAsync<BaseRequestPostForm, TResponse>(requestPostForm, cancellationToken),
            BaseRequestDelete requestDelete => await this.DeleteAsync<BaseRequestDelete, TResponse>(requestDelete, cancellationToken),
            _ => throw new NotSupportedException($"Not supported: {nameof(request)}")
        };
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

        if (this.apiOptions.CurrentValue.LogIn is not null)
        {
            var logInRootRequest = new LogInRootRequest
            {
                LogInRoot = new LogInRoot
                {
                    Username = this.apiOptions.CurrentValue.LogIn.Username,
                    Password = this.apiOptions.CurrentValue.LogIn.Password
                }
            };

            this.accessToken = await this.InvokeAsync<LogInRootRequest, AccessToken>(logInRootRequest, cancellationToken);

            return this.accessToken?.Token;
        }

        var jwtToken = this.httpContextAccessor.HttpContext?
            .GetJwtToken();

        return jwtToken ?? this.accessToken?.Token;
    }
    private async Task GetAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestGet
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }
    private async Task<TResponse?> GetAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestGet
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }
    private async Task PutAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPut
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null
            ? string.Empty
            : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }
    private async Task<TResponse?> PutAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPut
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null ? string.Empty : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }
    private async Task PostAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPost
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null
            ? string.Empty
            : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }
    private async Task<TResponse?> PostAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPost
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null
            ? string.Empty
            : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }
    private async Task PostFormAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPostForm
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var formContent = new MultipartFormDataContent();

        foreach (var x in request.GetForm())
        {
            await formContent
                .AddFormItem(x, cancellationToken);
        }

        httpRequest.Content = formContent;

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }
    private async Task<TResponse?> PostFormAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPostForm
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        using var formContent = new MultipartFormDataContent();

        foreach (var formItem in request.GetForm())
        {
            await formContent
                .AddFormItem(formItem, cancellationToken);
        }

        httpRequest.Content = formContent;

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }
    private async Task DeleteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestDelete
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null ? string.Empty : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        await GetResponseAsync(httpResponse, cancellationToken);
    }
    private async Task<TResponse?> DeleteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestDelete
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);

        var body = request.GetBody();
        var content = body == null ? string.Empty : JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

        using var httpResponse = await this.httpClient
            .SendAsync(httpRequest, cancellationToken);

        return await GetResponseAsync<TResponse>(httpResponse, cancellationToken);
    }

    private Uri GetUri<TRequest>(TRequest request)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var protocol = this.apiOptions.CurrentValue.UseSsl
            ? "https://"
            : "http://";
        var host = this.apiOptions.CurrentValue.Host.EndsWith("/", StringComparison.Ordinal)
            ? this.apiOptions.CurrentValue.Host[..^1]
            : this.apiOptions.CurrentValue.Host;
        var port = this.apiOptions.CurrentValue.Port;
        var root = this.apiOptions.CurrentValue.Root.EndsWith("/", StringComparison.Ordinal)
            ? this.apiOptions.CurrentValue.Root[..^1]
            : this.apiOptions.CurrentValue.Root;

        // BUG: To Annotations
        var controller = string.IsNullOrEmpty(request.Controller) ? null : $"{request.Controller}/";
        var action = string.IsNullOrEmpty(request.Action) ? null : $"{request.Action}/";

        var route = request.GetRoute();
        var queryString = request.GetQuerystring();
        var uri = $"{protocol}{host}:{port}/{root}/{controller}{action}{route}?{queryString}";

        return new Uri(uri);
    }
    private async Task<HttpRequestMessage> GetHttpRequestMessage<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var uri = this.GetUri(request);
        var method = GetMethod(request);
        var headers = request.GetHeaders();
        var jwtToken = await this.AuthenticateAsync(request, cancellationToken);

        var httpRequest = new HttpRequestMessage(method, uri);

        foreach (var header in headers)
        {
            httpRequest.Headers
                .Add(header.Key, header.Value);
        }

        if (jwtToken != null)
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        if (!string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name))
        {
            httpRequest.Headers.AcceptLanguage
                .Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.Name));
        }

        if (DateTimeInfo.TimeZone.Value != null)
        {
            httpRequest.Headers
                .Add(RequestTimeZoneHeaderProvider.Headerkey, DateTimeInfo.TimeZone.Value.Id);
        }

        return httpRequest;
    }

    private static string GetInferredController<TResponse>()
        where TResponse : class
    {
        var type = typeof(TResponse);

        return type.IsGenericType
            ? $"{type.GenericTypeArguments[0].Name}s"
            : $"{type.Name.ToLower()}s";
    }
    private static HttpMethod GetMethod<TRequest>(TRequest request)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        return request switch
        {
            BaseRequestGet => HttpMethod.Get,
            BaseRequestPut => HttpMethod.Put,
            BaseRequestPost => HttpMethod.Post,
            BaseRequestPostForm => HttpMethod.Post,
            BaseRequestDelete => HttpMethod.Delete,
            BaseRequestOptions => HttpMethod.Options,
            _ => throw new NotSupportedException()
        };
    }
    private static async Task GetResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
            case HttpStatusCode.NoContent:
                return;

            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();

            case HttpStatusCode.Forbidden:
                throw new PermissionDeniedException();

            case HttpStatusCode.BadRequest:
            {
                var errorContent = await httpResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                throw GetBadRequestException(errorContent);
            }

            case HttpStatusCode.InternalServerError:
            {
                var errorContent = await httpResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                var internalServerErrorException = GetInternalServerErrorException(errorContent);

                if (internalServerErrorException != null)
                {
                    throw internalServerErrorException;
                }

                break;
            }
        }

        httpResponse
            .EnsureSuccessStatusCode();
    }
    private static async Task<TResponse?> GetResponseAsync<TResponse>(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
            case HttpStatusCode.NoContent:
                return null;

            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();

            case HttpStatusCode.Forbidden:
                throw new PermissionDeniedException();

            case HttpStatusCode.BadRequest:
            {
                var errorContent = await httpResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                throw GetBadRequestException(errorContent);
            }

            case HttpStatusCode.InternalServerError:
            {
                var errorContent = await httpResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                var internalServerErrorException = GetInternalServerErrorException(errorContent);

                if (internalServerErrorException != null)
                {
                    throw internalServerErrorException;
                }

                break;
            }
        }

        httpResponse
            .EnsureSuccessStatusCode();

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

        return string.IsNullOrEmpty(content)
            ? null
            : JsonConvert.DeserializeObject<TResponse>(content, SerializerSettings.GetDefault());
    }
    private static BadRequestException GetBadRequestException(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content, SerializerSettings.GetDefault());

            if (problemDetails == null)
            {
                throw new NullReferenceException(nameof(problemDetails));
            }

            throw new ProblemDetailsException(problemDetails);
        }
        catch (JsonException)
        {
            if (content.StartsWith("\"", StringComparison.Ordinal))
            {
                content = content[1..];
            }

            if (content.EndsWith("\"", StringComparison.Ordinal))
            {
                content = content[..^1];
            }

            var exceptionMessage = content
                .RemoveQuotes();

            return new BadRequestException(exceptionMessage);
        }
    }
    private static InvalidOperationException GetInternalServerErrorException(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        try
        {
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content, SerializerSettings.GetDefault());

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

            return new InvalidOperationException(exceptionMessage);
        }
    }
}

/// <inheritdoc />
public abstract class BaseApi<TIdentity> : BaseApi
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseApi(IOptionsMonitor<ApiClientOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }

    /// <summary>
    /// Index.
    /// Invokes the 'index' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="IndexRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Get.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Get.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> GetAsync<TEntity>(TIdentity id, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Get many.
    /// Invokes the 'details/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Get many.
    /// Invokes the 'details/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Details.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Details Many.
    /// Invokes the 'details' endpoint of the api, with multiple id's.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public virtual Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/count' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryCountRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public virtual async Task<int> QueryCountAsync<TEntity, TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.InvokeAsync<TEntity, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

        int.TryParse(response, out var count);

        return count;
    }

    /// <summary>
    /// Create.
    /// Invokes the 'create' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<TEntity> CreateAsync<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.InvokeAsync<CreateRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NullReferenceException(nameof(entityCreated));
    }

    /// <summary>
    /// Create And Get.
    /// Invokes the 'create/get' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<TEntity?> CreateAndGetAsync<TEntity>(CreateAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.InvokeAsync<CreateAndGetRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NullReferenceException(nameof(entityCreated));
    }

    /// <summary>
    /// Create Many.
    /// Invokes the 'create/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Create Many Bulk.
    /// Invokes the 'create/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual Task CreateManyBulkAsync<TEntity>(CreateManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<CreateManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit.
    /// Invokes the 'edit' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<TEntity?> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Edit And Get.
    /// Invokes the 'edit/get' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<TEntity?> EditAndGetAsync<TEntity>(EditAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditAndGetRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many.
    /// Invokes the 'Edit/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many Bulk.
    /// Invokes the 'Edit/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual Task EditManyBulkAsync<TEntity>(EditManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Query.
    /// Invokes the 'edit/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="EditQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task EditQueryAsync<TEntity, TCriteria>(EditQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<EditQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete.
    /// Invokes the 'delete' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteAsync<TEntity>(DeleteRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Many.
    /// Invokes the 'delete/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteManyAsync<TEntity>(DeleteManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Many Bulk.
    /// Invokes the 'delete/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyBulkRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteManyBulkAsync<TEntity>(DeleteManyBulkRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteManyBulkRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Query.
    /// Invokes the 'delete/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="DeleteQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteQueryAsync<TEntity, TCriteria>(DeleteQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }
}