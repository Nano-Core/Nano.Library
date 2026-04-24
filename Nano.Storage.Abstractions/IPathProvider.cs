namespace Nano.Storage.Abstractions;

/// <summary>
/// Provides filesystem paths used by the storage subsystem.
/// </summary>
/// <remarks>
///     Implementations supply paths for the root of the storage share and for temporary file storage.
///     Returned paths must be valid, absolute, and suitable for file system operations.
/// </remarks>
public interface IPathProvider
{
    /// <summary>
    /// Gets the absolute root directory of the configured storage share.
    /// </summary>
    /// <remarks>
    ///     This path represents the base directory under which all storage operations should occur.
    /// </remarks>
    string Root { get; }

    /// <summary>
    /// Gets the absolute directory used for temporary file storage.
    /// </summary>
    /// <remarks>
    ///     This directory may be located outside the storage share and is intended for transient files such as uploads, processing artifacts, or buffering.
    /// </remarks>
    string Tmp { get; }
}