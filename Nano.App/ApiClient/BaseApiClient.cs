using Nano.App.ApiClient.Apis;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient;

/// <summary>
/// Base API client implementation for applications using the default <see cref="Guid"/> identity type.
/// </summary>
/// <param name="apiClient">The underlying API client used for communication with the server.</param>
public abstract class BaseApiClient(ApiClient apiClient)
    : BaseApiClient<Guid>(apiClient);

/// <summary>
/// Base API client implementation providing access to core Nano API groups and request invocation helpers.
/// </summary>
/// <typeparam name="TIdentity">The identity type used by the application.</typeparam>
public abstract class BaseApiClient<TIdentity>(ApiClient apiClient)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient apiClient = apiClient;

    /// <summary>
    /// Provides access to authentication-related API endpoints.
    /// </summary>
    public AuthApi Auth { get; } = new(apiClient);

    /// <summary>
    /// Provides access to audit-related API endpoints.
    /// </summary>
    public AuditApi<TIdentity> Audit { get; } = new(apiClient);

    /// <summary>
    /// Provides access to entity-related API endpoints.
    /// </summary>
    public EntityApi<TIdentity> Entity { get; } = new(apiClient);

    /// <summary>
    /// Invokes an API request that does not return a response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.apiClient.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Invokes an API request and returns a response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The request instance.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The response instance, or <c>null</c> if no response was returned.</returns>
    protected Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.apiClient.InvokeAsync<TRequest, TResponse>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TEntity">The entity.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    protected Task<TResponse?> InvokeAsync<TEntity, TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.apiClient
            .InvokeAsync<TEntity, TRequest, TResponse>(request, cancellationToken);
    }
}