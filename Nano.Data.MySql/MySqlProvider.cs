using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Config.Enums;
using Nano.Data.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.MySql;

/// <summary>
/// MySQL data provider using Pomelo.EntityFrameworkCore.MySql.
/// </summary>
/// <remarks>
///     Supports retry policies, batching, spatial data via NetTopologySuite, query splitting behavior, and optional health checks.
///     Documentation: https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data.MySql/README.md#nanodatamysql.
/// </remarks>
public sealed class MySqlProvider : IDataProvider
{
    private static readonly ConcurrentDictionary<string, MySqlDataSource> _dataSources = new();

    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<IDatabaseExceptionTranslator, MySqlExceptionTranslator>();

        if (options.HealthCheck != null)
        {
            var failureStatus = options.HealthCheck.UnhealthyStatus
                .GetHealthStatus();

            var healthChecksBuilder = services
                .AddHealthChecks();

            if (options.AuthenticationType == AuthenticationType.Azure)
            {
                var dataSource = GetOrCreateEntraDataSource(options.ConnectionString);

                healthChecksBuilder
                    .AddMySql(_ => dataSource, name: "mysql", failureStatus: failureStatus);
            }
            else
            {
                healthChecksBuilder
                    .AddMySql(options.ConnectionString, name: "mysql", failureStatus: failureStatus);
            }
        }
    }

    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var batchSize = options.BatchSize;
        var retryCount = options.QueryRetryCount;

        var connectionStringBuilder = new MySqlConnectionStringBuilder(options.ConnectionString)
        {
            AllowUserVariables = true,
            UseAffectedRows = false
        };

        var connectionString = connectionStringBuilder.ConnectionString;

        if (options.AuthenticationType == AuthenticationType.Azure)
        {
            var dataSource = GetOrCreateEntraDataSource(connectionString);

            using var connection = dataSource.CreateConnection();

            connection
                .Open();

            var serverVersion = ServerVersion.Parse(connection.ServerVersion);

            connection
                .Close();

            builder
                .UseMySql(dataSource, serverVersion, ConfigureMySql);
        }
        else
        {
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            builder
                .UseMySql(connectionString, serverVersion, ConfigureMySql);
        }

        void ConfigureMySql(MySqlDbContextOptionsBuilder mysqlBulder)
        {
            var querySplittingBehavior = options.QuerySplittingBehavior
                .GetQuerySplittingBehavior();

            mysqlBulder
                .MaxBatchSize(batchSize)
                .EnableRetryOnFailure(retryCount)
                .UseNetTopologySuite()
                .UseQuerySplittingBehavior(querySplittingBehavior);
        }
    }


    private static MySqlDataSource GetOrCreateEntraDataSource(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return _dataSources
            .GetOrAdd(connectionString, cs =>
            {
                var dataSourceBuilder = new MySqlDataSourceBuilder(cs);

                dataSourceBuilder
                    .UsePeriodicPasswordProvider(
                        (_, cancellationToken) => AzureEntraRdbmsTokenProvider.GetTokenAsync(cancellationToken),
                        TimeSpan.FromMinutes(50), TimeSpan.FromSeconds(10));

                return dataSourceBuilder
                    .Build();
            });
    }
}