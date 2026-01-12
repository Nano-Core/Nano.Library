using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public class DefaultAuthApi : BaseApi<Guid>
{
    /// <inheritdoc />
    protected DefaultAuthApi(IOptionsMonitor<ApiClientOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }
}