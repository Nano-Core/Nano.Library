using System;
using Nano.App.ApiClient.Apis;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <param name="apiClient"></param>
public abstract class BaseIdentityApiClient<TUser>(ApiClient apiClient) : BaseIdentityApiClient<TUser, Guid>(apiClient)
    where TUser : class, IEntityUser<Guid>;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseIdentityApiClient<TUser, TIdentity>(ApiClient apiClient) : BaseApiClient<TIdentity>(apiClient)
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    public IdentityApi<TUser, TIdentity> Identity { get; } = new(apiClient);
}