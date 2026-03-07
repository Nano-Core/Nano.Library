using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.App.Api.Mvc.Authorization;

/// <summary>
/// A custom authorization middleware result handler that allows requests to proceed without authentication if no authentication schemes are registered.
/// Otherwise, it delegates to the default <see cref="AuthorizationMiddlewareResultHandler"/>.
/// </summary>
public class OptionalAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler authorizationMiddlewareResultHandler = new();

    /// <summary>
    /// Handles the result of authorization for the current HTTP request.
    /// If no authentication schemes are registered, the request is allowed to continue without enforcing authorization. Otherwise, the default
    /// authorization handling is applied.
    /// </summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="context">The current <see cref="HttpContext"/> for the request.</param>
    /// <param name="policy">The <see cref="AuthorizationPolicy"/> that was evaluated.</param>
    /// <param name="authorizeResult">The result of evaluating the authorization policy.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(policy);
        ArgumentNullException.ThrowIfNull(authorizeResult);

        var schemes = context.RequestServices
            .GetRequiredService<IAuthenticationSchemeProvider>()
            .GetAllSchemesAsync()
            .GetAwaiter()
            .GetResult();

        if (!schemes.Any())
        {
            await next(context);

            return;
        }

        await this.authorizationMiddlewareResultHandler
            .HandleAsync(next, context, policy, authorizeResult);
    }
}