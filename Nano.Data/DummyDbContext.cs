using Microsoft.EntityFrameworkCore;

namespace Nano.Data
{
    /// <inheritdoc />
    public class DummyDbContext : DefaultDbContext
    {
        /// <inheritdoc />
        public DummyDbContext(DbContextOptions dbContextOptions, DataOptions dataOptions)
            : base(dbContextOptions, dataOptions)
        {

        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase("dummyDb");
        }
    }
}