using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;

namespace Nano.Data.PostgreSQL;

/// <summary>
/// PostgreSQL Data Provider.
/// </summary>
public sealed class PostgresProvider : IDataProvider
{
    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var batchSize = options.BatchSize;
        var retryCount = options.QueryRetryCount;
        var connectionString = options.ConnectionString;

        builder
            .UseNpgsql(connectionString, x =>
            {
                var querySplittingBehavior = options.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }

    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.HealthCheck == null)
        {
            return;
        }

        var failureStatus = options.HealthCheck.UnhealthyStatus
            .GetHealthStatus();

        services
            .AddHealthChecks()
            .AddNpgSql(options.ConnectionString, name: "postgres", failureStatus: failureStatus);
    }
}