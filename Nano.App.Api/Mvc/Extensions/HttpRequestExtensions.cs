using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Api.Mvc.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpRequest"/> to retrieve additional request information.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Retrieves the remote IP address of the client making the request.
    /// Checks the 'X-Forwarded-For' header for proxied requests.
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequest"/> instance.</param>
    /// <returns>The remote IP address as a string if available; otherwise, <c>null</c>. Returns the IPv4 address if the original IP is mapped to IPv6.</returns>
    public static string? GetRemoteIpAddress(this HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        var xForwardedFor = httpRequest.Headers["X-Forwarded-For"].ToString();

        if (string.IsNullOrEmpty(xForwardedFor))
        {
            return null;
        }

        var success = IPAddress.TryParse(xForwardedFor.Split(',')[0], out var remoteIpAddress);

        if (!success || remoteIpAddress == null)
        {
            return null;
        }

        if (remoteIpAddress.IsIPv4MappedToIPv6)
        {
            return remoteIpAddress
                .MapToIPv4()
                .ToString();
        }

        return remoteIpAddress
            .ToString();
    }
}