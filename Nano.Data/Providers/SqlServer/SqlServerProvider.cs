using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Interfaces;

namespace Nano.Data.Providers.SqlServer
{
    /// <summary>
    /// Sql Server Data Provider.
    /// </summary>
    public class SqlServerProvider : IDataProvider
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual DataOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="DataOptions"/>.</param>
        public SqlServerProvider(DataOptions options)
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
            //var useLazyLoading = this.Options.UseLazyLoading;
            var connectionString = this.Options.ConnectionString;

            if (connectionString == null)
                return;

            builder
                // FEATURE: .UseLazyLoadingProxies(useLazyLoading)
                .UseSqlServer(connectionString, x => x.MaxBatchSize(batchSize));
        }
    }
}