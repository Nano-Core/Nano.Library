using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Api.Mvc.Middleware;

/// <inheritdoc />
public class HttpRequestIdentifierMiddleware : IMiddleware
{
    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(next);

        httpContext.Response.Headers["RequestId"] = httpContext.TraceIdentifier;

        return next(httpContext);
    }
}