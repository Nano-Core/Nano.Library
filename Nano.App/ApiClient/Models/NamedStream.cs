using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Nano.App.ApiClient.Models;

/// <summary>
/// Represents a named stream with an associated name and underlying <see cref="Stream"/>.
/// Implements <see cref="IDisposable"/> to ensure the stream is properly disposed.
/// </summary>
public sealed class NamedStream : IDisposable
{
    /// <summary>
    /// The name of the stream.
    /// Only the file name part of the path is stored.
    /// </summary>
    [Required]
    public string Name
    {
        get;
        set => field = Path.GetFileName(value);
    } = null!;

    /// <summary>
    /// The underlying <see cref="Stream"/> associated with this named stream.
    /// </summary>
    [Required]
    public Stream Stream { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this.Stream
            .Dispose();
    }
}