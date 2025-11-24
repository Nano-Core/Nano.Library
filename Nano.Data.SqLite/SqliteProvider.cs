using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;
using System;

namespace Nano.Data.SqLite;

/// <summary>
/// Sql Lite Data Provider.
/// </summary>
public class SqliteProvider : IDataProvider
{
    private readonly IOptionsMonitor<DataOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public SqliteProvider(IOptionsMonitor<DataOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.options.CurrentValue.BatchSize;
        var connectionString = this.options.CurrentValue.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlite(connectionString, x =>
            {
                var querySplittingBehavior = this.options.CurrentValue.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }
}