using Nano.Data;

namespace Nano.Repository
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