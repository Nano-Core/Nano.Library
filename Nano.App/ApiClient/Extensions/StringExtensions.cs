using System;
using System.IO;
using Nano.App.ApiClient.Consts;

namespace Nano.App.ApiClient.Extensions;

/// <summary>
/// String Extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Remove Quotes.
    /// </summary>
    /// <param name="string">The string.</param>
    /// <returns>The string without quotes.</returns>
    public static string RemoveQuotes(this string @string)
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

    /// <summary>
    /// Get Http Content Type.
    /// Gets the http content type mathcing the extension og the passed <paramref name="filename"/>.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>The http content type.</returns>
    public static string GetHttpContentType(this string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        var extension = Path
            .GetExtension(filename)
            .ToLower();

        return extension switch
        {
            ".jpg" or ".jpeg" => HttpContentType.JPEG,
            ".bmp" => HttpContentType.BMP,
            ".html" => HttpContentType.HTML,
            ".json" => HttpContentType.JSON,
            ".pdf" => HttpContentType.PDF,
            ".png" => HttpContentType.PNG,
            ".txt" => HttpContentType.TEXT,
            ".xhtml" => HttpContentType.XHTML,
            ".xml" => HttpContentType.XML,
            ".zip" => HttpContentType.ZIP,
            _ => throw new ArgumentOutOfRangeException(nameof(filename), extension, $"The extension: {extension} of filename {filename} is invalid.")
        };
    }
}