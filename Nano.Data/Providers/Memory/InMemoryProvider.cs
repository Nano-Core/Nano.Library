using System;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Interfaces;

namespace Nano.Data.Providers.Memory
{
    /// <summary>
    /// In Memory Data Provider.
    /// </summary>
    public class InMemoryProvider : IDataProvider
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual DataOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="DataOptions"/>.</param>
        public InMemoryProvider(DataOptions options)
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

            //var useLazyLoading = this.Options.UseLazyLoading;
            var connectionString = this.Options.ConnectionString;

            if (connectionString == null)
                return;

            builder
                // FEATURE: .UseLazyLoadingProxies(useLazyLoading)
                .UseInMemoryDatabase(connectionString);
        }
    }
}