using Microsoft.EntityFrameworkCore;

namespace Nano.Data
{
    /// <summary>
    /// Base Db Context (abstract).
    /// </summary>
    public abstract class BaseDbContext : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Db context options.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
            
        }
    }
}