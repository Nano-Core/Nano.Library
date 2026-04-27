using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Nano.Data.SqLite;

/// <summary>
/// SQLite data provider.
/// </summary>
/// <remarks>
///     Intended for local development, lightweight deployments, and embedded database scenarios.
///     Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.Data.SqLite
/// </remarks>
public sealed class SqLiteProvider : IDataProvider
{
    /// <inheritdoc />
    public static void Configure(IServiceCollection services, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services
            .AddSingleton<IDatabaseExceptionTranslator, SqLiteExceptionTranslator>();

        if (options.HealthCheck != null)
        {
            var failureStatus = options.HealthCheck.UnhealthyStatus
                .GetHealthStatus();

            services
                .AddHealthChecks()
                .AddSqlite(options.ConnectionString, name: "sqlite", failureStatus: failureStatus);
        }
    }

    /// <inheritdoc />
    public static void Configure(DbContextOptionsBuilder builder, DataOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var batchSize = options.BatchSize;
        var connectionString = options.ConnectionString;

        builder
            .UseSqlite(connectionString, x =>
            {
                var querySplittingBehavior = options.QuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }
}