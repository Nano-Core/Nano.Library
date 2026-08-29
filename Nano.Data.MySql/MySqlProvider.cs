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

            services
                .AddHealthChecks()
                .AddMySql(options.ConnectionString, name: "mysql", failureStatus: failureStatus);
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
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        if (options.AuthenticationType == AuthenticationType.Azure)
        {
            const string DEFAULT_URL = "https://ossrdbms-aad.database.windows.net/.default";

            var credential = new WorkloadIdentityCredential();
            var dataSourceBuilder = new MySqlDataSourceBuilder(connectionString);

            dataSourceBuilder
                .UsePeriodicPasswordProvider(
                    async (_, cancellationToken) =>
                    {
                        var request = new TokenRequestContext([DEFAULT_URL]);

                        var token = await credential
                            .GetTokenAsync(request, cancellationToken);

                        return token.Token;
                    }, TimeSpan.FromMinutes(50), TimeSpan.FromSeconds(10));

            var dataSource = dataSourceBuilder
                .Build();

            builder
                .UseMySql(dataSource, serverVersion, ConfigureMySql);
        }
        else
        {
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
}