using System;
using Microsoft.AspNetCore.Http;
using Nano.Common.Consts;

namespace Nano.App.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequest"/>.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Gets the request id from the HTTP request.
    /// </summary>
    /// <param name="httpRequest">The current HTTP request.</param>
    /// <returns>The request id if present; otherwise, null.</returns>
    public static string? GetRequestId(this HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        httpRequest.Headers
            .TryGetValue(NanoHeaderNames.REQUEST_ID, out var values);

        if (string.IsNullOrEmpty(values))
        {
            return null;
        }

        return values.ToString();
    }
}