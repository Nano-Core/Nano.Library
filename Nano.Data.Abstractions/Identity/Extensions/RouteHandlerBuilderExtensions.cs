using System;
using System.Net;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nano.Common.Consts;

namespace Nano.Data.Abstractions.Identity.Extensions;

/// <summary>
/// Provides extension methods for configuring common defaults on <see cref="RouteHandlerBuilder"/> instances,
/// including versioning, authorization, CORS, metadata, and standardized response types.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Configures default settings for an endpoint without a request body.
    /// Applies response metadata, versioning, authorization, and documentation settings.
    /// </summary>
    /// <param name="builder">The route handler builder.</param>
    /// <param name="summary">The summary description used for documentation (e.g., Swagger).</param>
    /// <param name="tag">The tag used to group the endpoint in documentation.</param>
    /// <param name="version">The API version as a string (e.g., "1.0").</param>
    /// <param name="allowAnonymous">If <c>true</c>, the endpoint allows anonymous access; otherwise, authorization is required.</param>
    public static void WithEndpointDefaults(this RouteHandlerBuilder builder, string summary, string tag, string version, bool allowAnonymous)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(version);

        if (allowAnonymous)
        {
            builder
                .AllowAnonymous();
        }
        else
        {
            builder
                .RequireAuthorization();
        }

        var apiVersion = version
            .ToApiVersion();

        builder
            .ProducesProblem((int)HttpStatusCode.NotFound)
            .ProducesProblem((int)HttpStatusCode.Unauthorized)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .WithSummary(summary)
            .WithTags(tag)
            .WithApiVersionSet(new ApiVersionSetBuilder(apiVersion.ToString())
                .HasApiVersion(apiVersion)
                .Build())
            .MapToApiVersion(apiVersion);
    }

    /// <summary>
    /// Configures default settings for an endpoint that returns a response without a request body.
    /// Applies response metadata, versioning, authorization, and documentation settings.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response returned by the endpoint.</typeparam>
    /// <param name="builder">The route handler builder.</param>
    /// <param name="summary">The summary description used for documentation (e.g., Swagger).</param>
    /// <param name="tag">The tag used to group the endpoint in documentation.</param>
    /// <param name="version">The API version as a string (e.g., "1.0").</param>
    /// <param name="allowAnonymous">If <c>true</c>, the endpoint allows anonymous access; otherwise, authorization is required.</param>
    public static void WithEndpointDefaults<TResponse>(this RouteHandlerBuilder builder, string summary, string tag, string version, bool allowAnonymous)
        where TResponse : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(tag);
        ArgumentNullException.ThrowIfNull(version);

        builder
            .Produces<TResponse>()
            .WithEndpointDefaults(summary, tag, version, allowAnonymous);
    }

    /// <summary>
    /// Configures default settings for an endpoint that accepts a request body and returns a response.
    /// Applies request/response metadata, versioning, authorization, and documentation settings.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body accepted by the endpoint.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the endpoint.</typeparam>
    /// <param name="builder">The route handler builder.</param>
    /// <param name="summary">The summary description used for documentation (e.g., Swagger).</param>
    /// <param name="tag">The tag used to group the endpoint in documentation.</param>
    /// <param name="version">The API version as a string (e.g., "1.0").</param>
    /// <param name="allowAnonymous">If <c>true</c>, the endpoint allows anonymous access; otherwise, authorization is required.</param>
    public static void WithEndpointDefaults<TRequest, TResponse>(this RouteHandlerBuilder builder, string summary, string tag, string version, bool allowAnonymous)
        where TRequest : notnull
        where TResponse : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(tag);

        builder
            .Accepts<TRequest>(HttpContentType.JSON)
            .WithEndpointDefaults<TResponse>(summary, tag, version, allowAnonymous);
    }
}