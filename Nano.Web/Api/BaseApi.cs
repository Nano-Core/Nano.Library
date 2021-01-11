using Nano.Models.Exceptions;
using Nano.Models.Interfaces;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;
using Nano.Web.Api.Requests.Auth;
using Nano.Web.Api.Requests.Interfaces;
using Nano.Web.Const;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Serialization;
using Nano.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Web.Api.Requests;
using Nano.Web.Api.Requests.Spatial;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Providers;

namespace Nano.Web.Api
{
    /// <summary>
    /// Base Api (abstract).
    /// </summary>
    public abstract class BaseApi
    {
        private AccessToken accessToken;
        private readonly HttpClient httpClient;
        private readonly ApiOptions apiOptions;
        private readonly TimeSpan httpTimeout = new TimeSpan(0, 0, 30);
        private readonly HttpClientHandler httpClientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new EntityContractResolver()
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
                Timeout = this.httpTimeout,
                DefaultRequestVersion = new Version(2, 0)
            };

            this.httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));

            this.jsonSerializerSettings.Converters
                .Add(new StringEnumConverter());
        }

        /// <summary>
        /// Log-In Async.
        /// </summary>
        /// <param name="request">The <see cref="LogInRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> LogInAsync(LogInRequest request, CancellationToken cancellationToken = default)
        {
            return await this.CustomAsync<LogInRequest, AccessToken>(request, cancellationToken);
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

            return await this.CustomAsync<LogInRefreshRequest, AccessToken>(request, cancellationToken);
        }

        /// <summary>
        /// Log-In External Async.
        /// </summary>
        /// <param name="request">The <see cref="LogInRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<ExternalLoginResponse> LogInExternalAsync(LogInExternalRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<LogInExternalRequest, ExternalLoginResponse>(request, cancellationToken);
        }

        /// <summary>
        /// Log-Out Async.
        /// </summary>
        /// <param name="request">The <see cref="LogOutRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void..</returns>
        public virtual async Task LogOutAsync(LogOutRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.CustomAsync(request, cancellationToken);
        }

        /// <summary>
        /// Get External Schemes Async.
        /// </summary>
        /// <param name="request">The <see cref="GetExternalSchemesRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="LoginProvider"/>'s.</returns>
        public virtual async Task<IEnumerable<LoginProvider>> GetExternalSchemesAsync(GetExternalSchemesRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<GetExternalSchemesRequest, IEnumerable<LoginProvider>>(request, cancellationToken);
        }

        /// <summary>
        /// Invokes a custom request and returns void.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing (void).</returns>
        public virtual async Task CustomAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<TRequest, object>(request, cancellationToken);
        }

        /// <summary>
        /// Invokes a custom request and returns a response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
        public virtual async Task<TResponse> CustomAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<TRequest, TResponse>(request, cancellationToken);
        }

        /// <summary>
        /// Invokes the request, and returns the response.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
        protected virtual async Task<TResponse> Invoke<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.AuthenticateAsync(cancellationToken);

            var taskCompletion = new TaskCompletionSource<TResponse>();

            await this.ProcessRequestAsync<TRequest, TResponse>(request, cancellationToken)
                .ContinueWith(async x =>
                {
                    if (x.IsFaulted)
                    {
                        taskCompletion.SetException(x.Exception ?? new Exception());
                    }
                    else
                    {
                        try
                        {
                            var result = await x;
                            var response = await this.ProcessResponseAsync<TResponse>(result, cancellationToken);

                            taskCompletion.SetResult(response);
                        }
                        catch (Exception ex)
                        {
                            taskCompletion.SetException(ex);
                        }
                    }
                }, cancellationToken);

            return await taskCompletion.Task;
        }

        /// <summary>
        /// Invokes the request, and returns the response.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
        protected virtual async Task<TResponse> Invoke<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.AuthenticateAsync(cancellationToken);

            var taskCompletion = new TaskCompletionSource<TResponse>();

            await this.ProcessRequestAsync<TRequest, TEntity>(request, cancellationToken)
                .ContinueWith(async x =>
                {
                    if (x.IsFaulted)
                    {
                        taskCompletion.SetException(x.Exception ?? new Exception());
                    }
                    else
                    {
                        try
                        {
                            var result = await x;
                            var response = await this.ProcessResponseAsync<TResponse>(result, cancellationToken);

                            taskCompletion.SetResult(response);
                        }
                        catch (Exception ex)
                        {
                            taskCompletion.SetException(ex);
                        }
                    }
                }, cancellationToken);

            return await taskCompletion.Task;
        }

        private HttpMethod GetMethod<TRequest>(TRequest request)
        {
            return request switch
            {
                IRequestGet _ => HttpMethod.Get,
                IRequestPost _ => HttpMethod.Post,
                IRequestDelete _ => HttpMethod.Delete,
                _ => throw new NotSupportedException()
            };
        }
        private async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            var isAnonymous = HttpContextAccess.Current
                .GetIsAnonymous();

            if (isAnonymous)
            {
                if (this.apiOptions.Login == null)
                    return;

                if (!string.IsNullOrEmpty(this.accessToken?.Token) && !this.accessToken.IsExpired)
                    return;

                this.apiOptions.Login.IsRefreshable = false;

                var loginRequest = new LogInRequest
                {
                    Login = this.apiOptions.Login
                };

                using var httpResponse = await this.ProcessRequestAsync<LogInRequest, AccessToken>(loginRequest, cancellationToken);
                this.accessToken = await this.ProcessResponseAsync<AccessToken>(httpResponse, cancellationToken);
            }
        }
        private async Task<HttpResponseMessage> ProcessRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = request.GetUri<TResponse>(this.apiOptions);
            var method = this.GetMethod(request);
            var isAnonymous = HttpContextAccess.Current.GetIsAnonymous();

            var jwtToken = isAnonymous 
                ? this.accessToken?.Token 
                : HttpContextAccess.Current.GetJwtToken();

            var httpRequest = new HttpRequestMessage(method, uri);
            httpRequest.Headers.Add(RequestTimeZoneHeaderProvider.Headerkey, DateTimeInfo.TimeZone.Value.Id);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            httpRequest.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.Name));

            if (request is IRequestPost requestPost)
            {
                var body = requestPost.GetBody();
                var content = body == null ? string.Empty : JsonConvert.SerializeObject(body, this.jsonSerializerSettings);

                httpRequest.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);
            }

            return await this.httpClient
                .SendAsync(httpRequest, cancellationToken);
        }
        private async Task<TResponse> ProcessResponseAsync<TResponse>(HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            try
            {
                using (httpResponse)
                {
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            return default;

                        case HttpStatusCode.Unauthorized:
                            throw new AggregateException(new UnauthorizedException());

                        case HttpStatusCode.BadRequest:
                        case HttpStatusCode.InternalServerError:
                            var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                            var error = JsonConvert.DeserializeObject<Error>(errorContent);
                            
                            if (error.IsTranslated)
                            {
                                throw new AggregateException(error.Exceptions.Select(x => new TranslationException(x)));
                            }
                            else if (this.apiOptions.UseExposeErrors)
                            {
                                throw new AggregateException(error.Exceptions.Select(x => new InvalidOperationException(x)));
                            }
                            break;
                    }

                    httpResponse
                        .EnsureSuccessStatusCode();

                    if (typeof(TResponse) == typeof(object))
                    {
                        return default;
                    }

                    var successContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    return JsonConvert.DeserializeObject<TResponse>(successContent);
                }
            }
            finally
            {
                httpResponse.Dispose();
            }
        }
    }
  
    /// <inheritdoc />
    public class BaseApi<TIdentity> : BaseApi
    {
        /// <inheritdoc />
        public BaseApi(ApiOptions apiOptions)
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
        public virtual async Task<TEntity> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            return await this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
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
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            return await this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
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
        public virtual async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Details.
        /// Invokes the 'details' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entity.</returns>
        public virtual async Task<TEntity> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
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
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken);
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
            where TEntity: class, IEntity
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<TEntity> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TEntity: class, IEntity
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
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
            where TEntity: class, IEntity
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<TEntity, QueryCountRequest<TCriteria>, int>(request, cancellationToken);
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
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Create Many.
        /// Invokes the 'create/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The created entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit.
        /// Invokes the 'edit' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<TEntity> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditQueryAsync<TEntity>(EditQueryRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditQueryRequest, IEnumerable<TEntity>>(request, cancellationToken);
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

            await this.Invoke<DeleteRequest<TIdentity>, TEntity>(request, cancellationToken);
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

            await this.Invoke<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
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

            await this.Invoke<DeleteQueryRequest, TEntity>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> CoveredByAsync<TEntity, TCriteria>(CoveredByRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<CoveredByRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> CoversAsync<TEntity, TCriteria>(CoversRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<CoversRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> CrossesAsync<TEntity, TCriteria>(CrossesRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<CrossesRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> DisjointsAsync<TEntity, TCriteria>(DisjointsRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<DisjointsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> IntersectsAsync<TEntity, TCriteria>(IntersectsRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<IntersectsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> OverlapsAsync<TEntity, TCriteria>(OverlapsRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<OverlapsRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> TouchesAsync<TEntity, TCriteria>(TouchesRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<TouchesRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> WithinAsync<TEntity, TCriteria>(WithinRequest<TCriteria> request, CancellationToken cancellationToken = default)
            where TEntity: class, IEntitySpatial
            where TCriteria : IQueryCriteriaSpatial, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<WithinRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
        }
    }
}