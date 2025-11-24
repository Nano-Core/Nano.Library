using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using System;

namespace Nano.Data.InMemory;

/// <summary>
/// In Memory Data Provider.
/// </summary>
public class InMemoryProvider : IDataProvider
{
    private readonly IOptionsMonitor<DataOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public InMemoryProvider(IOptionsMonitor<DataOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (this.options.CurrentValue.ConnectionString == null)
        {
            return;
        }

        builder
            .UseInMemoryDatabase(this.options.CurrentValue.ConnectionString);
    }
}