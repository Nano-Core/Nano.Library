using System;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Nano.Data.Interfaces;

namespace Nano.Data.Providers.MySql;

/// <summary>
/// MySql Data Provider.
/// </summary>
public class MySqlProvider : IDataProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual DataOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public MySqlProvider(DataOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.Options.BatchSize;
        var retryCount = this.Options.QueryRetryCount;
        var connectionString = this.Options.ConnectionString;

        var connection = new MySqlConnection(connectionString);
        var serverVersion = ServerVersion.AutoDetect(connection);

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseMySql(connection, serverVersion, x =>
            {
                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
            });
    }
}