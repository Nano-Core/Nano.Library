using Nano.Data;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultServiceSpatial : BaseServiceSpatial<DefaultDbContext>
    {
        /// <inheritdoc />
        public DefaultServiceSpatial(DefaultDbContext context)
            : base(context)
        {

        }
    }
}