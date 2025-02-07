using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Interfaces;

namespace Nano.Data.Providers.SqlServer;

/// <summary>
/// Sql Server Data Provider.
/// </summary>
public class SqlServerProvider : IDataProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual DataOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public SqlServerProvider(DataOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.Options.BatchSize;
        var retryCount = this.Options.QueryRetryCount;
        var connectionString = this.Options.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlServer(connectionString, x =>
            {
                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(this.Options.UseQuerySplittingBehavior);
            });
    }
}