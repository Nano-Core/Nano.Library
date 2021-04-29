using System;
using Microsoft.EntityFrameworkCore;

namespace Nano.Data
{
    /// <inheritdoc />
    public class NullDbContext : DefaultDbContext
    {
        /// <inheritdoc />
        public NullDbContext(DbContextOptions<NullDbContext> dbContextOptions, DataOptions dataOptions)
            : base(dbContextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null) 
                throw new ArgumentNullException(nameof(optionsBuilder));

            optionsBuilder
                .UseInMemoryDatabase("nulldb");
        }
    }
}