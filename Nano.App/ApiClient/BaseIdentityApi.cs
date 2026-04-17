using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <param name="apiClient"></param>
public abstract class BaseIdentityApi<TUser>(ApiClient apiClient) : BaseIdentityApi<TUser, Guid>(apiClient)
    where TUser : class, IEntityUser<Guid>;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseIdentityApi<TUser, TIdentity>(ApiClient apiClient) : BaseApi<TIdentity>(apiClient)
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// 
    /// </summary>
    public IdentityApi<TUser, TIdentity> Identity { get; } = new(apiClient);
}