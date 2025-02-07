using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Interfaces;

namespace Nano.Data.Providers.Sqlite;

/// <summary>
/// Sql Lite Data Provider.
/// </summary>
public class SqliteProvider : IDataProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual DataOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public SqliteProvider(DataOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.Options.BatchSize;
        var connectionString = this.Options.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlite(connectionString, x =>
            {
                x.MaxBatchSize(batchSize);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(this.Options.UseQuerySplittingBehavior);
            });
    }
}