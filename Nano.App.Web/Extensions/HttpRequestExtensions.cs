using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Extensions;

/// <summary>
/// Http Request Extensions.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Get Remote IP-Address.
    /// From 'X-Forwarded-For' header
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
    /// <returns>The remote ip-address.</returns>
    public static string GetRemoteIpAddress(this HttpRequest httpRequest)
    {
        if (httpRequest == null)
            throw new ArgumentNullException(nameof(httpRequest));

        var xForwardedFor = httpRequest.Headers["X-Forwarded-For"].ToString();

        if (string.IsNullOrEmpty(xForwardedFor))
        {
            return null;
        }

        var success = IPAddress.TryParse(xForwardedFor.Split(',')[0], out var remoteIpAddress);

        if (!success)
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