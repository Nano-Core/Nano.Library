using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;

namespace Nano.Data.InMemory;

/// <summary>
/// In Memory Data Provider.
/// </summary>
public class InMemoryProvider : IDataProvider
{
    private readonly DataOptions options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public InMemoryProvider(DataOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (this.options.ConnectionString == null)
        {
            return;
        }

        builder
            .UseInMemoryDatabase(this.options.ConnectionString);
    }
}