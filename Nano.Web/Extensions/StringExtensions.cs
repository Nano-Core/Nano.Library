using System;
using System.IO;
using Nano.Web.Const;

namespace Nano.Web.Extensions;

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
        if (@string.StartsWith("\""))
        {
            @string = @string[1..];
        }

        if (@string.EndsWith("\""))
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
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));

        var extension = Path
            .GetExtension(filename)
            .ToLower();

        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return HttpContentType.JPEG;

            case ".bmp":
                return HttpContentType.BMP;

            case ".html":
                return HttpContentType.HTML;

            case ".json":
                return HttpContentType.JSON;

            case ".pdf":
                return HttpContentType.PDF;

            case ".png":
                return HttpContentType.PNG;

            case ".txt":
                return HttpContentType.TEXT;

            case ".xhtml":
                return HttpContentType.XHTML;

            case ".xml":
                return HttpContentType.XML;

            case ".zip":
                return HttpContentType.ZIP;

            default:
                throw new ArgumentOutOfRangeException(nameof(filename), extension, $"The extension: {extension} of filename {filename} is invalid.");
        }
    }
}