using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;

namespace Nano.Data.SqlServer;

/// <summary>
/// Sql Server Data Provider.
/// </summary>
public class SqlServerProvider : IDataProvider
{
    private readonly IOptionsMonitor<DataOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public SqlServerProvider(IOptionsMonitor<DataOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.options.CurrentValue.BatchSize;
        var retryCount = this.options.CurrentValue.QueryRetryCount;
        var connectionString = this.options.CurrentValue.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlServer(connectionString, x =>
            {
                var querySplittingBehavior = this.options.CurrentValue.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }
}