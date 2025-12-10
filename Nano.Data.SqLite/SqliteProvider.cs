using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;
using System;
using Nano.Common.Extensions;

namespace Nano.Data.SqLite;

/// <summary>
/// Sql Lite Data Provider.
/// </summary>
public class SqliteProvider : IDataProvider
{
    /// <inheritdoc />
    public virtual void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var batchSize = options.BatchSize;
        var connectionString = options.ConnectionString;

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseSqlite(connectionString, x =>
            {
                var querySplittingBehavior = options.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }

    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, DataOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (!options.UseHealthCheck)
        {
            return;
        }

        var failureStatus = options.UnhealthyStatus
            .GetHealthStatus();

        services
            .AddHealthChecks()
            .AddSqlite(options.ConnectionString, failureStatus: failureStatus);
    }
}