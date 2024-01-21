using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Hosting.Middleware;

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
                .Add("Access-Control-Allow-Origin", new[] { (string)httpContext.Request.Headers["Origin"] });

            if (corsPolicy.AllowAnyHeader)
            {
                headers
                    .Add("Access-Control-Allow-Headers", new[] { "*" });
            }
            else
            {
                var allowedHeaders = string.Join(", ", corsPolicy.Headers);

                headers
                    .Add("Access-Control-Allow-Headers", allowedHeaders);
            }

            if (corsPolicy.AllowAnyMethod)
            {
                headers
                    .Add("Access-Control-Allow-Methods", new[] { "*" });
            }
            else
            {
                var allowedMethods = string.Join(", ", corsPolicy.Methods);

                headers
                    .Add("Access-Control-Allow-Methods", allowedMethods);
            }

            headers
                .Add("Access-Control-Allow-Credentials", new[] { corsPolicy.SupportsCredentials.ToString() });

            response.StatusCode = (int)HttpStatusCode.OK;

            await response
                .WriteAsync("OK");

            return;
        }

        await next
            .Invoke(httpContext);
    }
}