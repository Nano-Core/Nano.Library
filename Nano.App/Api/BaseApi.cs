using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Extensions;
using Nano.App.Api.Requests;
using Nano.App.Api.Requests.Auth;
using Nano.App.Api.Requests.Spatial;
using Nano.App.Api.Responses;
using Nano.App.Extensions;
using Nano.Models;
using Nano.Models.Const;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Exceptions;
using Nano.Models.Interfaces;
using Nano.Models.Serialization.Const;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using StringWithQualityHeaderValue = System.Net.Http.Headers.StringWithQualityHeaderValue;

namespace Nano.App.Api;

/// <summary>
/// Base Api (abstract).
/// </summary>
public abstract class BaseApi : IDisposable
{
    private volatile AccessToken accessToken;

    private readonly ApiOptions apiOptions;
    private readonly HttpClient httpClient;
    private readonly HttpClientHandler httpClientHandler = new()
    {
        AllowAutoRedirect = true,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="apiOptions">The <see cref="ApiOptions"/>.</param>
    protected BaseApi(ApiOptions apiOptions)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));

        this.httpClient = new HttpClient(this.httpClientHandler)
        {
            Timeout = new TimeSpan(0, 0, this.apiOptions.TimeoutInSeconds),
            DefaultRequestVersion = new Version(2, 0)
        };

        this.httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));
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
    /// Get External Log-In Data Async.
    /// </summary>
    /// <param name="request">The <see cref="BaseGetExternalLoginDataRequest{TProvider}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<ExternalLogInData> GetExternalLogInDataAsync<TProvider>(BaseGetExternalLoginDataRequest<TProvider> request, CancellationToken cancellationToken = default)
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
    /// <returns>A collection of <see cref="LogInProvider"/>'s.</returns>
    public virtual Task<IEnumerable<LogInProvider>> GetExternalSchemesAsync(GetExternalSchemesRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<GetExternalSchemesRequest, IEnumerable<LogInProvider>>(request, cancellationToken);
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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        switch (request)
        {
            case BaseRequestGet requestGet:
                return this.GetAsync(requestGet, cancellationToken);

            case BaseRequestPut requestPut:
                return this.PutAsync(requestPut, cancellationToken);

            case BaseRequestPost requestPost:
                return this.PostAsync(requestPost, cancellationToken);

            case BaseRequestPostForm requestPostForm:
                return this.PostFormAsync(requestPostForm, cancellationToken);

            case BaseRequestDelete requestDelete:
                return this.DeleteAsync(requestDelete, cancellationToken);

            default:
                throw new NotSupportedException($"Not supported: {nameof(request)}");
        }
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    protected virtual async Task<TResponse> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller ??= this.GetInferredController<TResponse>();

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
    protected virtual async Task<TResponse> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TRequest : BaseRequest
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller ??= this.GetInferredController<TEntity>();

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

    private async Task<string> AuthenticateAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        if (typeof(TRequest) == typeof(LogInRequest))
        {
            return null;
        }

        if (request.JwtTokenOverride != null)
        {
            return request.JwtTokenOverride;
        }

        if (this.apiOptions.LogIn is { Username: not null, Password: not null })
        {
            var logInRequest = new LogInRequest
            {
                LogIn = new LogIn
                {
                    Username = this.apiOptions.LogIn.Username,
                    Password = this.apiOptions.LogIn.Password
                }
            };

            this.accessToken = await this.InvokeAsync<LogInRequest, AccessToken>(logInRequest, cancellationToken);

            return this.accessToken?.Token;
        }

        var httpContextAccessor = HttpContextAccesser.Current;

        var jwtToken = httpContextAccessor?
            .GetJwtToken();

        if (jwtToken != null)
        {
            return jwtToken;
        }

        if (!string.IsNullOrEmpty(this.accessToken?.Token) && !this.accessToken.IsExpired)
        {
            return this.accessToken?.Token;
        }

        return this.accessToken?.Token;
    }
    private async Task GetAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestGet
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            await this.GetResponseAsync(httpResponse, cancellationToken);
        }
    }
    private async Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestGet
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            return await this.GetResponseAsync<TResponse>(httpResponse, cancellationToken);
        }
    }
    private async Task PutAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPut
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null
                ? string.Empty
                : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            await this.GetResponseAsync(httpResponse, cancellationToken);
        }
    }
    private async Task<TResponse> PutAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPut
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null ? string.Empty : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            return await this.GetResponseAsync<TResponse>(httpResponse, cancellationToken);
        }
    }
    private async Task PostAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPost
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null
                ? string.Empty
                : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            await this.GetResponseAsync(httpResponse, cancellationToken);
        }
    }
    private async Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPost
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null
                ? string.Empty
                : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            return await this.GetResponseAsync<TResponse>(httpResponse, cancellationToken);
        }
    }
    private async Task PostFormAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPostForm
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            using var formContent = new MultipartFormDataContent();
            {
                foreach (var x in request.GetForm())
                {
                    await formContent
                        .AddFormItem(x, cancellationToken);
                }

                httpRequest.Content = formContent;

                var httpResponse = await this.httpClient
                    .SendAsync(httpRequest, cancellationToken);

                await this.GetResponseAsync(httpResponse, cancellationToken);
            }
        }
    }
    private async Task<TResponse> PostFormAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestPostForm
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            using var formContent = new MultipartFormDataContent();
            {
                foreach (var x in request.GetForm())
                {
                    await formContent
                        .AddFormItem(x, cancellationToken);
                }

                httpRequest.Content = formContent;

                var httpResponse = await this.httpClient
                    .SendAsync(httpRequest, cancellationToken);

                return await this.GetResponseAsync<TResponse>(httpResponse, cancellationToken);
            }
        }
    }
    private async Task DeleteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestDelete
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null ? string.Empty : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            await this.GetResponseAsync(httpResponse, cancellationToken);
        }
    }
    private async Task<TResponse> DeleteAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequestDelete
        where TResponse : class
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequest = await this.GetHttpRequestMessage(request, cancellationToken);
        {
            var body = request.GetBody();
            var content = body == null ? string.Empty : JsonSerializer.Serialize(body, Globals.jsonSerializerSettings);

            httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

            var httpResponse = await this.httpClient
                .SendAsync(httpRequest, cancellationToken);

            return await this.GetResponseAsync<TResponse>(httpResponse, cancellationToken);
        }
    }

    private Uri GetUri<TRequest>(TRequest request)
        where TRequest : BaseRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var protocol = this.apiOptions.UseSsl
            ? "https://"
            : "http://";
        var host = this.apiOptions.Host.EndsWith("/")
            ? this.apiOptions.Host[..^1]
            : this.apiOptions.Host;
        var port = this.apiOptions.Port;
        var root = this.apiOptions.Root.EndsWith("/")
            ? this.apiOptions.Root[..^1]
            : this.apiOptions.Root;
        var controller = string.IsNullOrEmpty(request.Controller) ? null : $"{request.Controller}/";
        var action = string.IsNullOrEmpty(request.Action) ? null : $"{request.Action}/";
        var route = request.GetRoute();
        var queryString = request.GetQuerystring();
        var uri = $"{protocol}{host}:{port}/{root}/{controller}{action}{route}?{queryString}";

        return new Uri(uri);
    }
    private string GetInferredController<TResponse>()
        where TResponse : class
    {
        var type = typeof(TResponse);

        return type.IsGenericType
            ? $"{type.GenericTypeArguments[0].Name}s"
            : $"{type.Name.ToLower()}s";
    }
    private HttpMethod GetMethod<TRequest>(TRequest request)
        where TRequest : BaseRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
    private async Task<HttpRequestMessage> GetHttpRequestMessage<TRequest>(TRequest request, CancellationToken cancellationToken)
        where TRequest : BaseRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var uri = this.GetUri(request);
        var method = this.GetMethod(request);
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
    private async Task GetResponseAsync(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return;

            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();

            case HttpStatusCode.Forbidden:
                throw new PermissionDeniedException();

            case HttpStatusCode.BadRequest:
                {
                    var errorContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    throw this.GetBadRequestException(errorContent);
                }

            case HttpStatusCode.InternalServerError:
                {
                    var errorContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var internalServerErrorException = this.GetInternalServerErrorException(errorContent);

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
    private async Task<TResponse> GetResponseAsync<TResponse>(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        switch (httpResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                return default;

            case HttpStatusCode.Unauthorized:
                throw new UnauthorizedException();

            case HttpStatusCode.Forbidden:
                throw new PermissionDeniedException();

            case HttpStatusCode.BadRequest:
                {
                    var errorContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    throw this.GetBadRequestException(errorContent);
                }

            case HttpStatusCode.InternalServerError:
                {
                    var errorContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var internalServerErrorException = this.GetInternalServerErrorException(errorContent);

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

            if (typeof(TResponse) == typeof(NamedStream))
            {
                var name = httpResponse.Content.Headers.ContentDisposition?.FileName;
                var namedStream = new NamedStream
                {
                    Name = name,
                    Stream = stream
                };

                return namedStream as TResponse;
            }

            return stream as TResponse;
        }

        var content = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        return JsonSerializer.Deserialize<TResponse>(content, Globals.jsonSerializerSettings);
    }
    private void SetAuthorizationHeader(string token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        var httpContext = HttpContextAccesser.Current;

        if (httpContext == null)
        {
            return;
        }

        httpContext.Request.Headers[HeaderNames.Authorization] = $"Bearer {token}";
    }
    private Exception GetBadRequestException(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        try
        {
            var error = JsonSerializer.Deserialize<Error>(content, Globals.jsonSerializerSettings);

            if (error == null)
            {
                throw new NullReferenceException(nameof(error));
            }

            var badRequestExceptions = error.Exceptions
                .Select(x => new BadRequestException(x));

            return new AggregateException(badRequestExceptions);
        }
        catch (JsonException)
        {
            if (content.StartsWith("\""))
            {
                content = content[1..];
            }

            if (content.EndsWith("\""))
            {
                content = content[..^1];
            }

            var exceptionMessage = content
                .RemoveQuotes();

            return new BadRequestException(exceptionMessage);
        }
    }
    private Exception GetInternalServerErrorException(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        try
        {
            var error = JsonSerializer.Deserialize<Error>(content, Globals.jsonSerializerSettings);

            if (error == null)
            {
                throw new NullReferenceException(nameof(error));
            }

            if (error.IsTranslated)
            {
                var translationExceptions = error.Exceptions
                    .Select(x => new TranslationException(x));

                throw new AggregateException(translationExceptions);
            }

            if (this.apiOptions.UseExposeErrors)
            {
                var invalidOperationExceptions = error.Exceptions
                    .Select(x => new InvalidOperationException(x));

                return new AggregateException(invalidOperationExceptions);
            }
        }
        catch (JsonException)
        {

            var exceptionMessage = content
                .RemoveQuotes();

            return new InvalidOperationException(exceptionMessage);
        }

        return null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.httpClient?
            .Dispose();

        this.httpClientHandler?
            .Dispose();
    }
}

/// <inheritdoc />
public abstract class BaseApi<TIdentity> : BaseApi
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseApi(ApiOptions apiOptions)
        : base(apiOptions)
    {

    }

    /// <summary>
    /// Get.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id
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
    /// Index.
    /// Invokes the 'index' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="IndexRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Details.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
    public virtual Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken);
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
    public virtual Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteira type</typeparam>
    /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public virtual Task<TEntity> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the api.
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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
    public virtual Task<TEntity> CreateAsync<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<CreateRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Create Many.
    /// Invokes the 'create/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entities.</returns>
    public virtual Task<IEnumerable<TEntity>> CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit.
    /// Invokes the 'edit' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<TEntity> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<EditRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many.
    /// Invokes the 'Edit/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entities.</returns>
    public virtual Task<IEnumerable<TEntity>> EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many.
    /// Invokes the 'Edit/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entities.</returns>
    public virtual Task<IEnumerable<TEntity>> EditQueryAsync<TEntity>(EditQueryRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<EditQueryRequest, IEnumerable<TEntity>>(request, cancellationToken);
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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await this.InvokeAsync<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Query.
    /// Invokes the 'delete/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteQueryAsync<TEntity>(DeleteQueryRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await this.InvokeAsync<DeleteQueryRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Covered-By.
    /// Invokes the 'covered-by' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="CoveredByRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> CoveredByAsync<TEntity, TCriteria>(CoveredByRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<CoveredByRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Covers.
    /// Invokes the 'covers' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="CoversRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> CoversAsync<TEntity, TCriteria>(CoversRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<CoversRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Crosses.
    /// Invokes the 'crosses' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="CrossesRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> CrossesAsync<TEntity, TCriteria>(CrossesRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<CrossesRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Disjoints.
    /// Invokes the 'disjoints' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="DisjointsRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> DisjointsAsync<TEntity, TCriteria>(DisjointsRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<DisjointsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Intersects.
    /// Invokes the 'intersects' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="IntersectsRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> IntersectsAsync<TEntity, TCriteria>(IntersectsRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<IntersectsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Overlaps.
    /// Invokes the 'overlaps' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="OverlapsRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> OverlapsAsync<TEntity, TCriteria>(OverlapsRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<OverlapsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Touches.
    /// Invokes the 'touches' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="TouchesRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> TouchesAsync<TEntity, TCriteria>(TouchesRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<TouchesRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Within.
    /// Invokes the 'within' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="WithinRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> WithinAsync<TEntity, TCriteria>(WithinRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : IQueryCriteriaSpatial, new()
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<WithinRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
    }
}