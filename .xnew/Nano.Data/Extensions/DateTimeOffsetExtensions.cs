using System;

namespace Nano.Models.Extensions;

/// <summary>
/// DateTimeOffset Extensions.
/// </summary>
public static class DateTimeOffsetExtensions
{
    private static readonly DateTime epoch = new(1970, 1, 1, 0, 0, 0);

    /// <summary>
    /// Gets the number of seconds since <see cref="epoch"/> (Unix).
    /// </summary>
    /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
    /// <returns>The number of seconds</returns>
    public static long GetEpochTime(this DateTimeOffset at)
    {
        return (long)(at - DateTimeOffsetExtensions.epoch).TotalMilliseconds;
    }
}