using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Interfaces;

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
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.Options = options;
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

            if (connectionString == null)
                return;

            builder
                .EnableSensitiveDataLogging(useSensitiveDataLogging)
                // TODO: .ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .UseLazyLoadingProxies(useLazyLoading)
                .UseMySql(connectionString, x =>
                {
                    x.MaxBatchSize(batchSize);
                    x.EnableRetryOnFailure(retryCount);
                });
        }
    }
}