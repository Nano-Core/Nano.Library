using System;
using System.IO;
using Microsoft.Extensions.Options;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage;

/// <summary>
/// Default implementation of <see cref="IPathProvider"/> that provides filesystem paths based on the configured <see cref="StorageOptions"/>.
/// </summary>
/// <remarks>
///     The <see cref="Root"/> represents the absolute base directory for storage operations and is derived from the configured logical container or share name in <see cref="StorageOptions"/>.
///     The <see cref="Tmp"/> provides a system-specific temporary directory for transient files such as uploads or processing artifacts.
/// </remarks>
public class PathProvider : IPathProvider
{
    private readonly IOptionsMonitor<StorageOptions> options;

    /// <inheritdoc />
    public virtual string Root => Path.Combine("/mnt", this.options.CurrentValue.ShareName);

    /// <inheritdoc />
    public virtual string Tmp => Path.GetTempPath();

    /// <summary>
    /// Initializes a new instance of <see cref="PathProvider"/>.
    /// </summary>
    /// <param name="options">A non-null <see cref="IOptionsMonitor{StorageOptions}"/> providing access to the configured storage share name.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public PathProvider(IOptionsMonitor<StorageOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }
}