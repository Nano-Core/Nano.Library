using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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
    public DefaultApi(IOptionsMonitor<ApiOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }
}