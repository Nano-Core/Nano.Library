using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models;
using Nano.Web.Api.Requests.Auth;
using Nano.Web.Api.Requests.Interfaces;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;

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

        private async Task Authenticate()
        {
            if (this.accessToken != null && this.accessToken.IsValid)
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
                case IRequestQuerystring _:
                    return await this.httpClient
                        .GetAsync(uri);

                case IRequestJson requestJson:
                    var body = requestJson.GetBody();
                    var content = JsonConvert.SerializeObject(body, this.jsonSerializerSettings);

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
                httpResponse
                    .EnsureSuccessStatusCode();

                var rawJson = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<TResponse>(rawJson);

                return response;
            }
        }
    }
}