using Nano.App.ApiClient.Config;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public class DefaultIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
    where TUser : class, IEntityUser<Guid>
{
    /// <inheritdoc />
    protected DefaultIdentityApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }
}