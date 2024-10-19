using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Consts;
using Nano.Security;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class DisableAuthControllerMiddleware : IMiddleware
{
    private readonly WebOptions webOptions;
    private readonly SecurityOptions securityOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="securityOptions"></param>
    public DisableAuthControllerMiddleware(WebOptions webOptions, SecurityOptions securityOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.securityOptions = securityOptions ?? throw new ArgumentNullException(nameof(securityOptions));
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        if (!this.securityOptions.IsAuth)
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