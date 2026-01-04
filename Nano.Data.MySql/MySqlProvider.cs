using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;

namespace Nano.Data.MySql;

/// <summary>
/// MySql Data Provider.
/// </summary>
public class MySqlProvider : IDataProvider
{
    /// <inheritdoc />
    public virtual void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var batchSize = options.BatchSize;
        var retryCount = options.QueryRetryCount;
        var connectionString = options.ConnectionString;

        var connection = new MySqlConnection(connectionString);
        var serverVersion = ServerVersion.AutoDetect(connection);

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseMySql(connection, serverVersion, x =>
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
            .AddMySql(options.ConnectionString, failureStatus: failureStatus);
    }
}