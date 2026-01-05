using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public abstract class DefaultAuthApi : BaseApi<Guid>
{
    /// <inheritdoc />
    protected DefaultAuthApi(IOptionsMonitor<ApiOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }
}