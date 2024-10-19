using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class DisableAuthControllerMiddleware : IMiddleware
{
    private readonly string rootPath;
    private readonly bool isAuthEnabled;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="isAuthEnabled"></param>
    public DisableAuthControllerMiddleware(string rootPath, bool isAuthEnabled)
    {
        this.isAuthEnabled = isAuthEnabled;
        this.rootPath = rootPath;
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        if (!this.isAuthEnabled)
        {
            if (httpContext.Request.Path.StartsWithSegments($"{this.rootPath}/auth"))
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                return Task.CompletedTask;
            }
        }

        return next(httpContext);
    }
}