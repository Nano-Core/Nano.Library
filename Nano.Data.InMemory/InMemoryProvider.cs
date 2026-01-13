using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using System;

namespace Nano.Data.InMemory;

/// <summary>
/// In Memory Data Provider.
/// </summary>
public class InMemoryProvider : IDataProvider
{
    /// <inheritdoc />
    public virtual void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder
            .UseInMemoryDatabase(options.ConnectionString);
    }

    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
    }
}