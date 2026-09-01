using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Config.Enums;
using Nano.Data.Extensions;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections.Concurrent;

namespace Nano.Data.PostgreSQL;

/// <summary>
/// PostgreSQL data provider using Npgsql.
/// </summary>
/// <remarks>
///     Supports retry policies, batching, spatial data via NetTopologySuite, vector similarity search via Pgvector, query splitting behavior, and optional health checks.
///     Documentation: https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data.PostgreSQL/README.md#nanodatapostgresql.
/// </remarks>
public sealed class PostgresSqlProvider : IDataProvider
{
    private static readonly ConcurrentDictionary<string, NpgsqlDataSource> _dataSources = new();

    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<IDatabaseExceptionTranslator, PostgreSqlExceptionTranslator>();

        if (options.HealthCheck != null)
        {
            var failureStatus = options.HealthCheck.UnhealthyStatus
                .GetHealthStatus();

            if (options.AuthenticationType == AuthenticationType.Azure)
            {
                var dataSource = GetOrCreateEntraDataSource(options.ConnectionString);

                services
                    .AddHealthChecks()
                    .AddNpgSql(_ => dataSource, name: "postgres", failureStatus: failureStatus);
            }
            else
            {
                services
                    .AddHealthChecks()
                    .AddNpgSql(options.ConnectionString, name: "postgres", failureStatus: failureStatus);
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
        var connectionString = options.ConnectionString;

        void ConfigureNpgsql(NpgsqlDbContextOptionsBuilder x)
        {
            var querySplittingBehavior = options.QuerySplittingBehavior
                .GetQuerySplittingBehavior();

            x.MaxBatchSize(batchSize);
            x.EnableRetryOnFailure(retryCount);
            x.UseNetTopologySuite();
            x.UseVector();
            x.UseQuerySplittingBehavior(querySplittingBehavior);
        }

        if (options.AuthenticationType == AuthenticationType.Azure)
        {
            builder
                .UseNpgsql(GetOrCreateEntraDataSource(connectionString), ConfigureNpgsql);
        }
        else
        {
            builder
                .UseNpgsql(connectionString, ConfigureNpgsql);
        }
    }


    private static NpgsqlDataSource GetOrCreateEntraDataSource(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        return _dataSources
            .GetOrAdd(connectionString, cs =>
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);

                dataSourceBuilder
                    .UseVector();

                dataSourceBuilder
                    .UsePeriodicPasswordProvider((_, cancellationToken) => AzureEntraRdbmsTokenProvider.GetTokenAsync(cancellationToken), TimeSpan.FromMinutes(50), TimeSpan.FromSeconds(10));

                return dataSourceBuilder
                    .Build();
            });
    }
}