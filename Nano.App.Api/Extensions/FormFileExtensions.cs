using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Api.Extensions;

/// <summary>
/// Extension methods for working with <see cref="IFormFile"/> instances.
/// </summary>
public static class FormFileExtensions
{
    /// <summary>
    /// Saves the contents of an <see cref="IFormFile"/> to the specified file path.
    /// </summary>
    /// <param name="file">The uploaded form file to persist to disk.</param>
    /// <param name="savePath">The full destination file path, including the file name.</param>
    /// <param name="cancellationToken">A token used to cancel the asynchronous copy operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="file"/> or <paramref name="savePath"/> is <c>null</c>.</exception>
    public static async Task SaveFileAsync(this IFormFile file, string savePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(savePath);

        await using var stream = new FileStream(savePath, FileMode.Create);

        await file
            .CopyToAsync(stream, cancellationToken);
    }
}