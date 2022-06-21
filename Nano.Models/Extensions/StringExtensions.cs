using System;

namespace Nano.Models.Extensions;

/// <summary>
/// String Extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns a substring consisting of all characters after the index of the passed <paramref name="indexOf"/>
    /// from the passed <paramref name="string"/>.
    /// </summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="indexOf">The <see cref="string"/> to find indexOf in the passed <paramref name="string"/>.</param>
    /// <returns>The sustring.</returns>
    public static string Substring(this string @string, string indexOf)
    {
        if (@string == null)
            throw new ArgumentNullException(nameof(@string));

        if (indexOf == null)
            throw new ArgumentNullException(nameof(indexOf));

        var index = @string.IndexOf(indexOf, StringComparison.Ordinal) + 1;
        var length = @string.Length - index;

        if (index < 0)
            return @string;

        return length < 0
            ? string.Empty
            : @string.Substring(index, length);
    }

    /// <summary>
    /// Tries to get a substring consisting of all characters after the index of the passed <paramref name="indexOf"/>
    /// from the passed <paramref name="string"/>.
    /// </summary>
    /// <param name="string">The <see cref="string"/>.</param>
    /// <param name="indexOf">The <see cref="string"/> to find indexOf in the passed <paramref name="string"/>.</param>
    /// <param name="result">The sustring.</param>
    /// <returns>A <see cref="bool"/> indicating success or not.</returns>
    public static bool TrySubstring(this string @string, string indexOf, out string result)
    {
        try
        {
            result = @string.Substring(indexOf);
        }
        catch
        {
            result = null;
        }

        return result != null;
    }
}