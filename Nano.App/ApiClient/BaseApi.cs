using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
public abstract class BaseApi(ApiClient apiClient) : BaseApi<Guid>(apiClient);

/// <summary>
/// 
/// </summary>
public abstract class BaseApi<TIdentity>(ApiClient apiClient)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient apiClient = apiClient;

    /// <summary>
    /// 
    /// </summary>
    public AuthApi Auth { get; } = new(apiClient);

    /// <summary>
    /// 
    /// </summary>
    public AuditApi<TIdentity> Audit { get; } = new(apiClient);

    /// <summary>
    /// 
    /// </summary>
    public EntityApi<TIdentity> Entity { get; } = new(apiClient);

    /// <summary>
    /// Invokes the request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    protected Task InvokeAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.apiClient
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the request, and returns the response.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <param name="request">The instance of type <typeparamref name="TRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TResponse"/>.</returns>
    protected Task<TResponse?> InvokeAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRequest
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.apiClient
            .InvokeAsync<TRequest, TResponse>(request, cancellationToken);
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
