using System;
using System.IO;

namespace Nano.Web.Api.Responses;

/// <summary>
/// Named Stream.
/// </summary>
public class NamedStream : IDisposable
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }

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