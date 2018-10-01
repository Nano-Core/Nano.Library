using Nano.Data;

namespace Nano.Services
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