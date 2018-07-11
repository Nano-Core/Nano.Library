using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            var useLazyLoading = this.Options.UseLazyLoading;
            var connectionString = this.Options.ConnectionString;

            if (connectionString == null)
                return;

            // TODO: Add caching...
            //builder
            //    .UseMemoryCache(new MemoryCache(new MemoryCacheOptions()));
            // other config params, like version??? we need to support that.

            builder
                .UseLazyLoadingProxies(useLazyLoading)
                .UseMySql(connectionString, x =>
                {
                    x.MaxBatchSize(batchSize);
                });
        }
    }
}