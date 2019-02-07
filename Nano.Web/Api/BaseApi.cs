using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models;
using Nano.Models.Auth;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests.Auth;
using Nano.Web.Api.Requests.Interfaces;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Exceptions;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        /// <param name="apiOptions">The <see cref="Api.ApiOptions"/>.</param>
        protected BaseApi(ApiOptions apiOptions)
        {
            this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));

            this.httpClient = new HttpClient(this.httpClientHandler)
            {
                Timeout = this.httpTimeout
            };

            this.httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));

            this.jsonSerializerSettings.Converters
                .Add(new StringEnumConverter());
        }

        /// <summary>
        /// Invokes a custom request and returns void.
        /// </summary>
        /// <typeparam name="TRequest">The request type.</typeparam>
        /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing (void).</returns>
        public virtual async Task Custom<TRequest>(TRequest request, CancellationToken cancellationToken = default)
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
        public virtual async Task<TResponse> Custom<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
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

            await this.Authenticate();

            var taskCompletion = new TaskCompletionSource<TResponse>();

            await this.ProcessRequestAsync<TRequest, TResponse>(request)
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
                            var response = await this.ProcessResponseAsync<TResponse>(result);

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

            await this.Authenticate();

            var taskCompletion = new TaskCompletionSource<TResponse>();

            await this.ProcessRequestAsync<TRequest, TEntity>(request)
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
                            var response = await this.ProcessResponseAsync<TResponse>(result);

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

        private async Task Authenticate()
        {
            if (this.accessToken != null && this.accessToken.IsExpired)
                return;

            var loginRequest = new LoginRequest
            {
                Login = this.apiOptions.Login
            };

            await this.ProcessRequestAsync<LoginRequest, AccessToken>(loginRequest)
                .ContinueWith(async x =>
                {
                    var result = await x;
                    var accessToken = await this.ProcessResponseAsync<AccessToken>(result);

                    this.accessToken = accessToken;
                    this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.accessToken.Token);
                });
        }
        private async Task<HttpResponseMessage> ProcessRequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = request.GetUri<TResponse>(this.apiOptions);

            switch (request)
            {
                case IRequestGet _:
                    return await this.httpClient
                        .GetAsync(uri);

                case IRequestDelete _:
                    var bodyDelete = string.Empty;
                    using (var stringContent = new StringContent(bodyDelete, Encoding.UTF8, HttpContentType.JSON))
                    {
                        var httpDeleteMessage = new HttpRequestMessage(HttpMethod.Delete, uri)
                        {
                            Content = stringContent
                        };

                        using (httpDeleteMessage)
                        {
                            return await this.httpClient
                                .SendAsync(httpDeleteMessage);
                        }
                    }

                case IRequestPost requestPost:
                    var bodyPost = requestPost.GetBody();
                    var content = JsonConvert.SerializeObject(bodyPost, this.jsonSerializerSettings);

                    using (var stringContent = new StringContent(content, Encoding.UTF8, HttpContentType.JSON))
                    {
                        return await this.httpClient
                            .PostAsync(uri, stringContent);
                    }
                    
                default:
                    throw new NotSupportedException();
            }
        }
        private async Task<TResponse> ProcessResponseAsync<TResponse>(HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            using (httpResponse)
            {
                var rawJson = await httpResponse.Content
                    .ReadAsStringAsync();

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return default;

                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.InternalServerError:
                        var error = JsonConvert.DeserializeObject<Error>(rawJson);
                        var message = error.Exceptions.FirstOrDefault() ?? error.Summary;

                        if (error.TranslationCode > 0)
                        {
                            throw new TranslationException(error.TranslationCode, message);
                        }
                        else if (this.apiOptions.UseExposeErrors)
                        {
                            throw new InvalidOperationException(message);
                        }
                        break;
                }

                httpResponse
                    .EnsureSuccessStatusCode();

                var response = JsonConvert.DeserializeObject<TResponse>(rawJson);

                return response;
            }
        }
    }
}