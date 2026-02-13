using Microsoft.AspNetCore.Http;
using Nano.Common.Consts;
using System;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Middleware;

/// <summary>
/// Middleware that adds the <c>RequestId</c> header to HTTP responses using the <see cref="HttpContext.TraceIdentifier"/>.
/// </summary>
public sealed class HttpRequestIdentifierMiddleware : IMiddleware
{
    /// <summary>
    /// Adds the <c>RequestId</c> header to the response and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <param name="next">The next <see cref="RequestDelegate"/> to invoke.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(next);

        if (!httpContext.Response.Headers.ContainsKey(NanoHeaderNames.REQUEST_ID))
        {
            httpContext.Response.Headers[NanoHeaderNames.REQUEST_ID] = httpContext.TraceIdentifier;
        }

        return next(httpContext);
    }
}