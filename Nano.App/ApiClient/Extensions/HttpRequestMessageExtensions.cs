using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Requests;
using Nano.Common.Consts;
using Nano.Common.Serialization.Json;
using Newtonsoft.Json;
using Vivet.AspNetCore.RequestTimeZone;
using Vivet.AspNetCore.RequestTimeZone.Providers;

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

        if (!string.IsNullOrEmpty(CultureInfo.CurrentCulture.Name))
        {
            httpRequestMessage.Headers.AcceptLanguage
                .Add(new StringWithQualityHeaderValue(CultureInfo.CurrentCulture.Name));
        }

        if (DateTimeInfo.TimeZone.Value != null)
        {
            httpRequestMessage.Headers
                .Add(RequestTimeZoneHeaderProvider.Headerkey, DateTimeInfo.TimeZone.Value.Id);
        }

        if (jwtToken != null)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        if (requestIdHeader != null)
        {
            httpRequestMessage.Headers
                .Add(NanoHeaderNames.REQUEST_ID, requestIdHeader);
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