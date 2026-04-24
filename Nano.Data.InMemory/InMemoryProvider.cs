using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using System;

namespace Nano.Data.InMemory;

/// <summary>
/// In-memory data provider intended for testing and lightweight scenarios.
/// </summary>
/// <remarks>
///     This provider uses Entity Framework Core's in-memory database and does not register additional services such as health checks.
///     Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.InMemory
/// </remarks>
public sealed class InMemoryProvider : IDataProvider
{
    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
    }

    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder
            .UseInMemoryDatabase(options.ConnectionString);
    }
}