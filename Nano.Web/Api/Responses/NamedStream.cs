using System.IO;

namespace Nano.Web.Api.Responses;

/// <summary>
/// Named Stream.
/// </summary>
public class NamedStream
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Stream.
    /// </summary>
    public virtual Stream Stream { get; set; }
}