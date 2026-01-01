using System;
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
    public virtual string Name
    {
        get;
        set => field = Path.GetFileName(value);
    }

    /// <summary>
    /// Stream.
    /// </summary>
    public virtual Stream Stream { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Stream?
            .Dispose();
    }
}