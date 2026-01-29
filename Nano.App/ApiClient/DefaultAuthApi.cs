using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Nano.App.ApiClient.Config;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public class DefaultAuthApi : BaseApi<Guid>
{
    /// <inheritdoc />
    protected DefaultAuthApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }
}