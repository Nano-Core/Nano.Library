using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Requests;
using Nano.Common.Consts;
using Nano.Common.Serialization.Json;
using Newtonsoft.Json;

namespace Nano.App.ApiClient.Extensions;

internal static class HttpRequestMessageExtensions
{
    internal static async Task AddHttpHeaders<TRequest>(this HttpRequestMessage httpRequestMessage, TRequest request, string? jwtToken = null, string? requestIdHeader = null, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        await Task.CompletedTask;

        var headers = request
            .GetHeaders();

        foreach (var header in headers)
        {
            httpRequestMessage.Headers
                .Add(header.Key, header.Value);
        }
    }

    internal static async Task AddHttpBody<TRequest>(this HttpRequestMessage httpRequestMessage, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        await Task.CompletedTask;

        var body = request
            .GetBody();

        if (body == null)
        {
            return;
        }

        var content = JsonConvert.SerializeObject(body, SerializerSettings.GetDefault());

        httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, HttpContentType.JSON);
    }

    internal static async Task AddHttpForm<TRequest>(this HttpRequestMessage httpRequestMessage, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        var form = request
            .GetForm();

        if (form.Any())
        {
            using var formContent = new MultipartFormDataContent();

            foreach (var x in request.GetForm())
            {
                await formContent
                    .AddFormItem(x, cancellationToken);
            }

            httpRequestMessage.Content = formContent;
        }
    }
}