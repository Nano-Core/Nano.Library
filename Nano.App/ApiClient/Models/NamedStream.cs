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
    public required string Name
    {
        get;
        set => field = Path.GetFileName(value);
    }

    /// <summary>
    /// The underlying <see cref="Stream"/> associated with this named stream.
    /// </summary>
    [Required]
    public required Stream Stream { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Stream
            .Dispose();
    }
}