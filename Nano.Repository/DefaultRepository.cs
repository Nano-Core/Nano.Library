using Nano.Data;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultRepository : BaseRepository<DefaultDbContext>
    {
        /// <inheritdoc />
        public DefaultRepository(DefaultDbContext context)
            : base(context)
        {

        }
    }
}