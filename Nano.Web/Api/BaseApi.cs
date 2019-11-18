using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private HttpMethod GetMethod<TRequest>(TRequest request)
        {
            switch (request)
            {
                case IRequestGet _:
                    return HttpMethod.Get;

                case IRequestPost _:
                    return HttpMethod.Post;

                case IRequestDelete _:
                    return HttpMethod.Delete;
            }

            throw new NotSupportedException();
        }
        private async Task AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            var jwtToken = HttpContextAccess.Current
                .GetJwtToken();

            if (jwtToken != null)
                return;

            if (this.apiOptions.Login == null)
                return;

            if (this.accessToken != null && !this.accessToken.IsExpired)	
                return;
               
            var loginRequest = new LogInRequest
            {
                Login = this.apiOptions.Login
            };

            using (var httpResponse = await this.ProcessRequestAsync<LogInRequest, AccessToken>(loginRequest, cancellationToken))
            {
                this.accessToken = await this.ProcessResponseAsync<AccessToken>(httpResponse);	
            }
        }
        private async Task<HttpResponseMessage> ProcessRequestAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class, IRequest
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var uri = request.GetUri<TResponse>(this.apiOptions);
            var method = this.GetMethod(request);
            var jwtToken = HttpContextAccess.Current.GetJwtToken() ?? this.accessToken?.Token;

            var httpRequst = new HttpRequestMessage(method, uri);
            httpRequst.Headers.Add(RequestTimeZoneHeaderProvider.Headerkey, DateTimeInfo.TimeZone.Value.Id);
            httpRequst.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            httpRequst.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.Name));

            if (request is IRequestPost requestPost)
            {
                var body = requestPost.GetBody();
                var content = body == null ? string.Empty : JsonConvert.SerializeObject(body, this.jsonSerializerSettings);
                var stringContent = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);

                httpRequst.Content = stringContent;
            }

            return await this.httpClient
                .SendAsync(httpRequst, cancellationToken);
        }
        private async Task<TResponse> ProcessResponseAsync<TResponse>(HttpResponseMessage httpResponse)
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
                            var errorContent = await httpResponse.Content.ReadAsStringAsync();
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
                        .ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TResponse>(successContent);
                }
            }
            finally
            {
                httpResponse.Dispose();
            }
        }
    }
}