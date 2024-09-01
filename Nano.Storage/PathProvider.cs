using System;
using System.IO;
using Nano.Storage.Interfaces;

namespace Nano.Storage;

/// <summary>
/// Path Provider.
/// </summary>
public class PathProvider : IPathProvider
{
    /// <summary>
    /// Storage Options.
    /// </summary>
    protected internal StorageOptions StorageOptions { get; }

    /// <inheritdoc />
    public virtual string RootDir => Path.Combine("/mnt/", this.StorageOptions.ShareName);

    /// <inheritdoc />
    public virtual string TempDir => Path.GetTempPath();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="storageOptions">The <see cref="Storage.StorageOptions"/>.</param>
    public PathProvider(StorageOptions storageOptions)
    {
        this.StorageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
    }
}