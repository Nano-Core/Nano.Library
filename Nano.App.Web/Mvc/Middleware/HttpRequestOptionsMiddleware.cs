using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Web.Mvc.Middleware;

/// <inheritdoc />
public class HttpRequestOptionsMiddleware : IMiddleware
{
    private readonly ICorsPolicyProvider corsOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="corsOptions">The <see cref="ICorsPolicyProvider"/>.</param>
    public HttpRequestOptionsMiddleware(ICorsPolicyProvider corsOptions)
    {
        this.corsOptions = corsOptions ?? throw new ArgumentNullException(nameof(corsOptions));
    }

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        if (next == null)
            throw new ArgumentNullException(nameof(next));

        if (httpContext.Request.Method == HttpMethods.Options)
        {
            var response = httpContext.Response;
            var headers = response.Headers;

            var corsPolicy = await this.corsOptions
                .GetPolicyAsync(httpContext, null);

            if (corsPolicy == null)
            {
                throw new NullReferenceException(nameof(corsPolicy));
            }

            headers
                .TryAdd("Access-Control-Allow-Origin", new[] { (string)httpContext.Request.Headers["Origin"] });

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