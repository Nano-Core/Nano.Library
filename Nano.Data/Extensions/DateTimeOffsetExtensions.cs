using System;

namespace Nano.Data.Extensions;

/// <summary>
/// DateTimeOffset Extensions.
/// </summary>
public static class DateTimeOffsetExtensions
{
    private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0);

    internal static long GetEpochTime(this DateTimeOffset at)
    {
        return (long)(at - DateTimeOffsetExtensions.epoch).TotalMilliseconds;
    }
}