using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nano.Common.Consts;

namespace Nano.Common.Extensions;

/// <summary>
/// Provides extension methods for string values.
/// </summary>
public static class StringExtensions
{
    private static readonly IReadOnlyDictionary<string, string> extensionToContentType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { FileExtensions.JPG, HttpContentType.JPEG },
        { FileExtensions.JPEG, HttpContentType.JPEG },
        { FileExtensions.PNG, HttpContentType.PNG },
        { FileExtensions.BMP, HttpContentType.BMP },
        { FileExtensions.GIF, HttpContentType.GIF },
        { FileExtensions.TIFF, HttpContentType.TIFF },
        { FileExtensions.ICON, HttpContentType.ICON },
        { FileExtensions.SVG, HttpContentType.SVG },

        // Documents
        { FileExtensions.HTML, HttpContentType.HTML },
        { FileExtensions.XHTML, HttpContentType.XHTML },
        { FileExtensions.TXT, HttpContentType.TEXT },
        { FileExtensions.MD, HttpContentType.TEXT },
        { FileExtensions.CSV, HttpContentType.CSV },
        { FileExtensions.JSON, HttpContentType.JSON },
        { FileExtensions.XML, HttpContentType.XML },
        { FileExtensions.PDF, HttpContentType.PDF },

        // Archives
        { FileExtensions.ZIP, HttpContentType.ZIP },

        // Microsoft Office
        { FileExtensions.DOC, HttpContentType.WORD },
        { FileExtensions.DOCX, HttpContentType.WORD_OPEN_FORMAT },
        { FileExtensions.XLSX, HttpContentType.EXCEL_OPEN_FORMAT },
        { FileExtensions.XLS, HttpContentType.EXCEL },
        { FileExtensions.PPTX, HttpContentType.POWERPOINT_OPEN_FORMAT },
        { FileExtensions.PPT, HttpContentType.POWERPOINT },

        // Audio
        { FileExtensions.WAV, HttpContentType.WAV },
        { FileExtensions.WMA, HttpContentType.WMA },
        { FileExtensions.REAL_AUDIO, HttpContentType.REAL_AUDIO },
        { FileExtensions.MPEG, HttpContentType.VIDOE_MPEG },

        // Video
        { FileExtensions.MP4, HttpContentType.MP4 },
        { FileExtensions.QUICK_TIME, HttpContentType.QUICK_TIME },
        { FileExtensions.WMV, HttpContentType.WMV },
        { FileExtensions.MS_VIDEO, HttpContentType.MS_VIDEO },
        { FileExtensions.FLV, HttpContentType.FLV },
        { FileExtensions.WEB_M, HttpContentType.WEB_M },

        // Binary
        { FileExtensions.OCTET_STREAM, HttpContentType.OCTET_STREAM }
    };

    /// <summary>
    /// Gets the HTTP content type corresponding to the file extension of the specified filename.
    /// </summary>
    /// <param name="filename">The filename from which to determine the HTTP content type.</param>
    /// <returns>A string representing the HTTP content type associated with the file extension.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the file extension is not supported.</exception>
    public static string GetHttpContentType(this string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        var extension = Path.GetExtension(filename)
            .ToLowerInvariant();

        if (extension == null)
        {
            throw new ArgumentOutOfRangeException(nameof(filename), "Filename has no extension.");
        }

        if (extensionToContentType.TryGetValue(extension, out var contentType))
        {
            return contentType;
        }

        throw new ArgumentOutOfRangeException(nameof(filename), extension, $"The extension: {extension} of filename {filename} is invalid or unsupported.");
    }

    /// <summary>
    /// Gets the canonical file extension (from <see cref="FileExtensions"/>) for a given HTTP content type.
    /// </summary>
    /// <param name="contentType">The HTTP content type.</param>
    /// <returns>The canonical file extension, including the leading dot.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the content type is not mapped to any extension.</exception>
    public static string GetFileExtension(this string contentType)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        foreach (var kvp in extensionToContentType.Where(x => string.Equals(x.Value, contentType, StringComparison.OrdinalIgnoreCase)))
        {
            return kvp.Key;
        }

        throw new ArgumentOutOfRangeException(nameof(contentType), contentType, $"The content type {contentType} has no mapped file extension.");
    }
}