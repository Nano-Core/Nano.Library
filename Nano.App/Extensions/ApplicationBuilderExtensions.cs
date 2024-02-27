using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.App.Extensions;

/// <summary>
/// Application Builder Extensions.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="IHttpContextAccessor"/> middleware, and initializes the current <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpContextAccessor(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var httpContextAccessor = applicationBuilder.ApplicationServices
            .GetRequiredService<IHttpContextAccessor>();

        HttpContextAccessor.Configure(httpContextAccessor);

        return applicationBuilder;
    }
}