using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;
using Nano.Data.Interfaces;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Nano.Data.Providers.MySql
{
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
            var useLazyLoading = this.Options.UseLazyLoading;
            var useSensitiveDataLogging = this.Options.UseSensitiveDataLogging;
            var connectionString = this.Options.ConnectionString;

            var connection = new MySqlConnection(connectionString);
            var serverVersion = ServerVersion.AutoDetect(connection);

            if (connectionString == null)
                return;

            builder
                .EnableSensitiveDataLogging(useSensitiveDataLogging)
                .ConfigureWarnings(x =>
                {
                    x.Ignore(RelationalEventId.BoolWithDefaultWarning);
                    x.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
                })
                .UseLazyLoadingProxies(useLazyLoading)
                .UseMySql(connection, serverVersion, x =>
                {
                    x.MaxBatchSize(batchSize);
                    x.EnableRetryOnFailure(retryCount);
                    x.CharSet(CharSet.Utf8Mb4);
                    // x.UseNetTopologySuite(); // TODO: UseNetTopologySuite, Waiting for stable release of Pomelo.
                });
        }
    }
}