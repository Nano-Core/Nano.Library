namespace Nano.Storage.Interfaces;

/// <summary>
/// Path Provider interface.
/// </summary>
public interface IPathProvider
{
    /// <summary>
    /// Root Dir.
    /// </summary>
    string RootDir { get; }

    /// <summary>
    /// Temp Dir.
    /// </summary>
    string TempDir { get; }
}