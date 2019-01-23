using Microsoft.EntityFrameworkCore;

namespace Nano.Data
{
    /// <inheritdoc />
    public class NullDbContext : DefaultDbContext
    {
        /// <inheritdoc />
        public NullDbContext(DbContextOptions dbContextOptions, DataOptions dataOptions)
            : base(dbContextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}