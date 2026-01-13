using System;

namespace Nano.App.Api.Mvc.Extensions;

internal static class VersionExtensions
{
    internal static Version ParseVersion(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value == string.Empty)
        {
            return new Version(1, 0);
        }

        if (Version.TryParse(value, out var version))
        {
            return version;
        }

        if (int.TryParse(value, out var major))
        {
            return new Version(major, 0);
        }

        throw new FormatException($"Invalid version format: '{value}'. Expected 'major.minor'.");
    }
}