using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Config.Enums;
using Nano.Data.Extensions;
using System;

namespace Nano.Data.SqlServer;

/// <summary>
/// SQL Server data provider.
/// </summary>
/// <remarks>
///     Supports retry policies, batching, spatial data via NetTopologySuite, query splitting behavior, and optional health checks.
///     Documentation: https://github.com/Nano-Core/Nano.Library/blob/master/Nano.Data.SqlServer/README.md#nanodatasqlserver.
/// </remarks>
public sealed class SqlServerProvider : IDataProvider
{
    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<IDatabaseExceptionTranslator, SqlServerExceptionTranslator>();

        if (options.HealthCheck != null)
        {
            var failureStatus = options.HealthCheck.UnhealthyStatus
                .GetHealthStatus();

            var connectionString = GetConnectionString(options);

            services
                .AddHealthChecks()
                .AddSqlServer(connectionString, name: "sqlserver", failureStatus: failureStatus);
        }
    }

    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var batchSize = options.BatchSize;
        var retryCount = options.QueryRetryCount;
        var connectionString = GetConnectionString(options);

        builder
            .UseSqlServer(connectionString, x =>
            {
                var querySplittingBehavior = options.QuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }


    private static string GetConnectionString(DataOptions options)
    {
        if (options.AuthenticationType != AuthenticationType.Azure)
        {
            return options.ConnectionString;
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder(options.ConnectionString)
        {
            Authentication = SqlAuthenticationMethod.ActiveDirectoryWorkloadIdentity
        };

        return connectionStringBuilder.ConnectionString;
    }
}