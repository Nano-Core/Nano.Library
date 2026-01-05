using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public abstract class DefaultIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
    where TUser : class, IEntityUser<Guid>
{
    /// <inheritdoc />
    protected DefaultIdentityApi(IOptionsMonitor<ApiOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }
}