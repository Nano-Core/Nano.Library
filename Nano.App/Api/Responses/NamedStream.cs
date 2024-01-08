using System;
using System.IO;

namespace Nano.App.Api.Responses;

/// <summary>
/// Named Stream.
/// </summary>
public class NamedStream : IDisposable
{
    private string name;

    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name
    {
        get => this.name;
        set => this.name = Path.GetFileName(value);
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