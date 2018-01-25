using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.Web.Api.Exceptions;
using Nano.Web.Api.Requests.Interfaces;
using Nano.Web.Api.Responses.Interfaces;
using Newtonsoft.Json;

namespace Nano.Web.Api
{
    /// <summary>
    /// http Facade.
    /// </summary>
    /// <typeparam name="TRequest">The type of <see cref="IRequest"/>.</typeparam>
    /// <typeparam name="TResponse">The type of <see cref="IResponse"/>.</typeparam>
    internal sealed class HttpFacade<TRequest, TResponse>
        where TRequest : IRequest, new()
        where TResponse : IResponse, new()
    {
        internal readonly TimeSpan defaultTimeout = new TimeSpan(0, 0, 30);
        internal static readonly HttpFacade<TRequest, TResponse> instance = new HttpFacade<TRequest, TResponse>();

        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>The <see cref="IResponse"/>.</returns>
        public TResponse Query(TRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return this.Query(request, this.defaultTimeout);
        }

        /// <summary>
        /// Query.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The <see cref="IResponse"/>.</returns>
        public TResponse Query(TRequest request, TimeSpan timeout)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return this.QueryAsync(request).Result;
        }

        /// <summary>
        /// Query Async.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>The <see cref="Task{TResponse}"/>.</returns>
        public Task<TResponse> QueryAsync(TRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return this.QueryAsync(request, CancellationToken.None);
        }

        /// <summary>
        /// Query Async.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The <see cref="Task{TResponse}"/>.</returns>
        public Task<TResponse> QueryAsync(TRequest request, TimeSpan timeout)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            return this.QueryAsync(request, timeout, CancellationToken.None);
        }

        /// <summary>
        /// Query Async.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Task{TResponse}"/>.</returns>
        public Task<TResponse> QueryAsync(TRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            return this.QueryAsync(request, TimeSpan.FromMilliseconds(Timeout.Infinite), cancellationToken);
        }

        /// <summary>
        /// Query Async.
        /// </summary>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Task{TResponse}"/>.</returns>
        public Task<TResponse> QueryAsync(TRequest request, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (timeout == null)
                throw new ArgumentNullException(nameof(timeout));

            if (cancellationToken == null)
                throw new ArgumentNullException(nameof(cancellationToken));

            var uri = request.GetUri();
            var httpClient = new HttpClient { Timeout = timeout };
            var taskCompletionSource = new TaskCompletionSource<TResponse>();

            Task<HttpResponseMessage> task;
            if (request is IRequestJson)
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var serializeObject = JsonConvert.SerializeObject(request, jsonSerializerSettings);

                using (var json = new StringContent(serializeObject, Encoding.UTF8))
                {
                    var content = json.ReadAsByteArrayAsync().Result;
                    var streamContent = new StreamContent(new MemoryStream(content));

                    task = httpClient.PostAsync(uri, streamContent, cancellationToken);
                }
            }
            else
                task = httpClient.GetAsync(uri, cancellationToken);

            task.ContinueWith(x =>
            {
                var response = default(TResponse);
                try
                {
                    if (x.IsCanceled)
                    {
                        taskCompletionSource.SetCanceled();
                    }
                    else if (x.IsFaulted)
                    {
                        var exception = x.Exception == null
                            ? new NullReferenceException("task.Exception")
                            : x.Exception?.InnerException ?? x.Exception ?? new Exception("unknown error");

                        throw exception;
                    }
                    else
                    {
                        var result = x.Result;
                        var content = result.Content;
                        var rawJson = content.ReadAsStringAsync().Result;
                        var rawBuffer = content.ReadAsByteArrayAsync().Result;

                        using (var stream = new MemoryStream(rawBuffer))
                        {
                            using (var streamReader = new StreamReader(stream))
                            {
                                var serializer = new JsonSerializer();
                                var reader = new JsonTextReader(streamReader);

                                try
                                {
                                    response = serializer.Deserialize<TResponse>(reader);
                                }
                                catch
                                {
                                    response = new TResponse();
                                }
                            }
                        }

                        response.RawJson = rawJson;
                        response.RawQueryString = result.RequestMessage.RequestUri.PathAndQuery;

                        result.EnsureSuccessStatusCode();

                        taskCompletionSource.SetResult(response);
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex is ApiException ? ex : new ApiException(ex.Message, ex) { Response = response };

                    taskCompletionSource.SetException(exception);
                }
                finally
                {
                    httpClient.Dispose();
                }
            }, TaskContinuationOptions.ExecuteSynchronously);

            return taskCompletionSource.Task;
        }
    }
}