using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions.Config;
using Nano.Web.Controllers;

namespace Nano.Web.Hosting.Middleware;

/// <inheritdoc />
public class DisableAuditControllerMiddleware : IMiddleware
{
    private readonly IOptionsMonitor<WebOptions> webOptions;
    private readonly IOptionsMonitor<DataOptions> dataOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webOptions">The <see cref="WebOptions"/>.</param>
    /// <param name="dataOptions">The <see cref="DataOptions"/>.</param>
    public DisableAuditControllerMiddleware(IOptionsMonitor<WebOptions> webOptions, IOptionsMonitor<DataOptions> dataOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.dataOptions = dataOptions ?? throw new ArgumentNullException(nameof(dataOptions));
    }

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        if (this.dataOptions.CurrentValue.ConnectionString == null || !this.webOptions.CurrentValue.Hosting.ExposeAuditController)
        {
            var controllerRoute = nameof(AuditController).Replace("Controller", string.Empty);

            if (httpContext.Request.Path.StartsWithSegments($"/{this.webOptions.CurrentValue.Hosting.Root}/{controllerRoute}"))
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                return Task.CompletedTask;
            }
        }

        return next(httpContext);
    }
}