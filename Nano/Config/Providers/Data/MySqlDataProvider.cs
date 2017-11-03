using System;
using Microsoft.EntityFrameworkCore;
using Nano.Config.Providers.Data.Interfaces;

namespace Nano.Config.Providers.Data
{
    /// <summary>
    /// MySql Data Provider.
    /// </summary>
    public class MySqlDataProvider : IDataProvider
    {
        /// <inheritdoc />
        public void Configure(DbContextOptionsBuilder builder, DataOptions options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var batchSize = options.BatchSize;
            var connectionString = options.ConnectionString;

            builder
                .UseMySql(connectionString, x => x.MaxBatchSize(batchSize));
        }
    }
}