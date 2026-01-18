using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using System;

namespace Nano.Data.InMemory;

/// <summary>
/// In Memory Data Provider.
/// </summary>
public sealed class InMemoryProvider : IDataProvider
{
    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder
            .UseInMemoryDatabase(options.ConnectionString);
    }

    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
    }
}