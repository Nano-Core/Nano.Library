using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;

namespace Nano.Data.SqlServer;

/// <summary>
/// Sql Server Data Provider.
/// </summary>
public class SqlServerProvider : IDataProvider
{
    private readonly DataOptions options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public SqlServerProvider(DataOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.options.BatchSize;
        var retryCount = this.options.QueryRetryCount;
        var connectionString = this.options.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlServer(connectionString, x =>
            {
                var querySplittingBehavior = this.options.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }
}