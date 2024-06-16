using System;
using Microsoft.AspNetCore.Http;

namespace Nano.App;

/// <summary>
/// Http Context Access (Static)
/// </summary>
public static class HttpContextAccessor
{
    private static IHttpContextAccessor accessor;

    /// <summary>
    /// Current.
    /// The current <see cref="HttpContext"/>, fetched through the <see cref="IHttpContextAccessor"/>.
    /// </summary>
    public static HttpContext Current => accessor?.HttpContext;

    /// <summary>
    /// Configure.
    /// </summary>
    /// <param name="httpContextAccessor">The <inheritdoc cref="IHttpContextAccessor"/>.</param>
    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor.accessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
}