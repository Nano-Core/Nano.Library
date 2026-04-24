using System;
using Nano.App.ApiClient.Apis;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <summary>
/// Base API client implementation for identity-enabled applications using a default <see cref="Guid"/> identity type.
/// </summary>
/// <typeparam name="TUser">The user type used by the application.</typeparam>
/// <param name="apiClient">The underlying API client used for communication with the server.</param>
public abstract class BaseIdentityApiClient<TUser>(ApiClient apiClient)
    : BaseIdentityApiClient<TUser, Guid>(apiClient)
    where TUser : class, IEntityUser<Guid>;

/// <summary>
/// Base API client implementation for identity-enabled applications supporting a custom identity type.
/// </summary>
/// <typeparam name="TUser">The user type used by the application.</typeparam>
/// <typeparam name="TIdentity">The identity type used to uniquely identify users.</typeparam>
public abstract class BaseIdentityApiClient<TUser, TIdentity>(ApiClient apiClient)
    : BaseApiClient<TIdentity>(apiClient)
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Provides access to identity-related API endpoints.
    /// </summary>
    public IdentityApi<TUser, TIdentity> Identity { get; } = new(apiClient);
}