using Microsoft.EntityFrameworkCore;

namespace Nano.Data
{
    /// <inheritdoc />
    public class DefaultDbContext : BaseDbContext
    {
        /// <inheritdoc />
        public DefaultDbContext(DbContextOptions options) 
            : base(options)
        {

        }
    }
}