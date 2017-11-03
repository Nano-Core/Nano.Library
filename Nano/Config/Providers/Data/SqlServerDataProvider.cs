using System;
using Microsoft.EntityFrameworkCore;
using Nano.Config.Providers.Data.Interfaces;

namespace Nano.Config.Providers.Data
{
    /// <summary>
    /// Sql Server Data Provider.
    /// </summary>
    public class SqlServerDataProvider : IDataProvider
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
                .UseSqlServer(connectionString, x => x.MaxBatchSize(batchSize));
        }
    }
}