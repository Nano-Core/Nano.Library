using System;
using System.IO;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage;

/// <summary>
/// Path Provider.
/// </summary>
public class PathProvider : IPathProvider
{
    private readonly StorageOptions options;

    /// <inheritdoc />
    public virtual string RootDir => Path.Combine("/mnt/", this.options.ShareName);

    /// <inheritdoc />
    public virtual string TempDir => Path.GetTempPath();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    public PathProvider(StorageOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }
}