using System;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Nano.Data.Abstractions.Config;
using Nano.Data.Extensions;
using Nano.Data.Interfaces;

namespace Nano.Data.MySql;

/// <summary>
/// MySql Data Provider.
/// </summary>
public class MySqlProvider : IDataProvider
{
    private readonly DataOptions options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="DataOptions"/>.</param>
    public MySqlProvider(DataOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var batchSize = this.options.BatchSize;
        var retryCount = this.options.QueryRetryCount;
        var connectionString = this.options.ConnectionString;

        var connection = new MySqlConnection(connectionString);
        var serverVersion = ServerVersion.AutoDetect(connection);

        if (connectionString == null)
        {
            return;
        }

        builder
            .UseMySql(connection, serverVersion, x =>
            {
                var querySplittingBehavior = this.options.UseQuerySplittingBehavior
                    .GetQuerySplittingBehavior();

                x.MaxBatchSize(batchSize);
                x.EnableRetryOnFailure(retryCount);
                x.UseNetTopologySuite();
                x.UseQuerySplittingBehavior(querySplittingBehavior);
            });
    }
}