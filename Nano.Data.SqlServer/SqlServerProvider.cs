using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;
using System;
using Nano.Common.Mvc.HealthChecks.Extensions;

namespace Nano.Data.SqlServer;

/// <summary>
/// Sql Server Data Provider.
/// </summary>
public sealed class SqlServerProvider : IDataProvider
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
            .UseSqlServer(connectionString, x =>
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
            .AddSqlServer(options.ConnectionString, name: "sqlserver", failureStatus: failureStatus);
    }
}