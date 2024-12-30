using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Consts;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class DisableAuthControllerMiddleware : IMiddleware
{
    private readonly WebOptions webOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webOptions">The <see cref="WebOptions"/>.</param>
    public DisableAuthControllerMiddleware(WebOptions webOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        if (!this.webOptions.Hosting.ExposeAuthController)
        {
            if (httpContext.Request.Path.StartsWithSegments($"/{this.webOptions.Hosting.Root}/{Constants.AUTH_CONTROLLER_ROUTE}")) 
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                return Task.CompletedTask;
            }
        }

        return next(httpContext);
    }
}