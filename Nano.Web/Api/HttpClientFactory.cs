using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Nano.Web.Const;

namespace Nano.Web.Api;

/// <summary>
///   Provide a default global httpClient and a factory a factory method
/// </summary>
public static class HttpClientFactory
{
    /// <summary>
    /// Configure Default Http Client.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
    /// <param name="options">The <see cref="ApiOptions"/>.</param>
    public static void ConfigureDefaultHttpClient(HttpClient httpClient, ApiOptions options)
    {
        if (httpClient == null)
            throw new ArgumentNullException(nameof(httpClient));

        httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutInSeconds);
        httpClient.DefaultRequestVersion = new Version(2, 0);

        httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));
    }

    /// <summary>
    /// Get Default Http Client Handler.
    /// </summary>
    /// <returns>The <see cref="HttpMessageHandler"/>.</returns>
    public static HttpClientHandler GetDefaultHttpClientHandler()
    {
        var httpClientHandler = new HttpClientHandler
        {
            AllowAutoRedirect = true
        };

        if (httpClientHandler.SupportsAutomaticDecompression)
        {
            httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        return httpClientHandler;
    }
}