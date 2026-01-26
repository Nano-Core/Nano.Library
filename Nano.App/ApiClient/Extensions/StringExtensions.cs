using System;

namespace Nano.App.ApiClient.Extensions;

internal static class StringExtensions
{
    internal static string RemoveQuotes(this string @string)
    {
        ArgumentNullException.ThrowIfNull(@string);

        if (@string.StartsWith("\"", StringComparison.Ordinal))
        {
            @string = @string[1..];
        }

        if (@string.EndsWith("\"", StringComparison.Ordinal))
        {
            @string = @string[..^1];
        }

        return @string;
    }
}