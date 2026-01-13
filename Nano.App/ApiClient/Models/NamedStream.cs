using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Nano.App.ApiClient.Models;

/// <summary>
/// Named Stream.
/// </summary>
public class NamedStream : IDisposable
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name
    {
        get;
        set => field = Path.GetFileName(value);
    } = null!;

    /// <summary>
    /// Stream.
    /// </summary>
    [Required]
    public virtual Stream Stream { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this.Stream?
            .Dispose();
    }
}