using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Api.Mvc.Middleware;

/// <summary>
/// Middleware to handle HTTP OPTIONS requests, typically used for CORS preflight requests.
/// Sets the appropriate CORS headers and returns a 200 OK response without invoking the next middleware.
/// </summary>
public sealed class HttpRequestOptionsMiddleware : IMiddleware
{
    private readonly ICorsPolicyProvider corsOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequestOptionsMiddleware"/> class.
    /// </summary>
    /// <param name="corsOptions">The <see cref="ICorsPolicyProvider"/> used to obtain CORS policies.</param>
    public HttpRequestOptionsMiddleware(ICorsPolicyProvider corsOptions)
    {
        this.corsOptions = corsOptions ?? throw new ArgumentNullException(nameof(corsOptions));
    }

    /// <summary>
    /// Invokes the middleware to handle OPTIONS requests and apply CORS headers.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="next">The next <see cref="RequestDelegate"/> in the pipeline.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(next);

        // BUG: 111: Review if this is correct

        if (httpContext.Request.Method == HttpMethods.Options)
        {
            var response = httpContext.Response;
            var headers = response.Headers;

            var corsPolicy = await this.corsOptions
                .GetPolicyAsync(httpContext, null);

            if (corsPolicy == null)
            {
                return;
            }

            headers
                .TryAdd("Access-Control-Allow-Origin", new[] { httpContext.Request.Headers["Origin"].ToString() });

            if (corsPolicy.AllowAnyHeader)
            {
                headers
                    .TryAdd("Access-Control-Allow-Headers", new[] { "*" });
            }
            else
            {
                var allowedHeaders = string.Join(", ", corsPolicy.Headers);

                headers
                    .TryAdd("Access-Control-Allow-Headers", allowedHeaders);
            }

            if (corsPolicy.AllowAnyMethod)
            {
                headers
                    .TryAdd("Access-Control-Allow-Methods", new[] { "*" });
            }
            else
            {
                var allowedMethods = string.Join(", ", corsPolicy.Methods);

                headers
                    .TryAdd("Access-Control-Allow-Methods", allowedMethods);
            }

            headers
                .TryAdd("Access-Control-Allow-Credentials", new[] { corsPolicy.SupportsCredentials.ToString() });

            response.StatusCode = (int)HttpStatusCode.OK;

            await response
                .WriteAsync("OK");

            return;
        }

        await next
            .Invoke(httpContext);
    }
}