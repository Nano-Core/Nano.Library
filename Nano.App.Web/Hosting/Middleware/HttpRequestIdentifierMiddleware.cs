using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class HttpRequestIdentifierMiddleware : IMiddleware
{
    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        httpContext.Response.Headers["RequestId"] = httpContext.TraceIdentifier;

        return next(httpContext);
    }
}