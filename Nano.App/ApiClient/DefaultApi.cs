using Microsoft.AspNetCore.Http;
using Nano.App.ApiClient.Config;
using System;
using System.Net.Http;

namespace Nano.App.ApiClient;

/// <summary>
/// Default Api.
/// </summary>
public class DefaultApi : BaseApi<Guid>
{
    /// <inheritdoc />
    public DefaultApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }
}