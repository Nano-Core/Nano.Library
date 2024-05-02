using System;
using Asp.Versioning;
using Nano.Config;

namespace Nano.Web.Extensions;

/// <summary>
/// Api Version Extensions.
/// </summary>
public static class ApiVersionExtensions
{
    /// <summary>
    /// Is Default.
    /// </summary>
    /// <returns></returns>
    public static bool IsDefault(this ApiVersion apiVersion)
    {
        if (apiVersion == null)
            throw new ArgumentNullException(nameof(apiVersion));

        var defaultVersion = new ApiVersion(ConfigManager.Version.Major, ConfigManager.Version.Minor);

        return defaultVersion == apiVersion;
    }
}