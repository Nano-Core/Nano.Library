using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public class DefaultIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
    where TUser : class, IEntityUser<Guid>
{
    /// <inheritdoc />
    protected DefaultIdentityApi(IOptionsMonitor<ApiClientOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }
}