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
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        builder
            .UseInMemoryDatabase(options.ConnectionString);
    }

    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, DataOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));
    }
}