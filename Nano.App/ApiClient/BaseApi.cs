using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using Nano.Data.Abstractions.Models.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Requests.Auth.Models;

namespace Nano.App.ApiClient;

/// <summary>
/// Base Api (abstract).
/// </summary>
public abstract class BaseApi
{
    private volatile AccessToken? accessToken;

    private readonly ApiClientOptions apiOptions;
    private readonly HttpClient httpClient;

    /// <summary>
    /// The httpcontext accessor.
    /// Use to acecss headers and jwt token to pass through the api-client request.
    /// </summary>
    protected internal readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="apiClientOptions">The <see cref="ApiClientOptions"/>.</param>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/>.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
    protected BaseApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        this.apiOptions = apiClientOptions ?? throw new ArgumentNullException(nameof(apiClientOptions));
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
    protected virtual async Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
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
    protected virtual async Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
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
    protected virtual async Task<TResponse?> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
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

        var protocol = this.apiOptions.UseSsl
            ? "https://"
            : "http://";
        var host = this.apiOptions.Host.EndsWith('/')
            ? this.apiOptions.Host[..^1]
            : this.apiOptions.Host;
        var port = this.apiOptions.Port;
        var root = this.apiOptions.Root.EndsWith('/')
            ? this.apiOptions.Root[..^1]
            : this.apiOptions.Root;

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

        var httpRequestMessage = new HttpRequestMessage(method, uri);

        await httpRequestMessage
            .AddHttpHeaders(request, jwtToken, cancellationToken);

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

        if (this.apiOptions.LogIn is not null)
        {
            var logInRootRequest = new LogInRootRequest
            {
                LogInRoot = new LogInRoot
                {
                    Username = this.apiOptions.LogIn.Username,
                    Password = this.apiOptions.LogIn.Password
                }
            };

            this.accessToken = await this.InvokeAsync<LogInRootRequest, AccessToken>(logInRootRequest, cancellationToken);

            return this.accessToken?.Token;
        }

        var jwtToken = this.httpContextAccessor.HttpContext?
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
    protected BaseApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
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