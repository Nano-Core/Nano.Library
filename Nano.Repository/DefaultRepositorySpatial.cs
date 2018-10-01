using Nano.Data;

namespace Nano.Repository
{
    /// <inheritdoc />
    public class DefaultRepositorySpatial : BaseRepositorySpatial<DefaultDbContext>
    {
        /// <inheritdoc />
        public DefaultRepositorySpatial(DefaultDbContext context)
            : base(context)
        {

        }
    }
}