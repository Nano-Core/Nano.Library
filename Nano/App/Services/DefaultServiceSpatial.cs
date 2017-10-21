using Nano.Data.Interfaces;

namespace Nano.App.Services
{
    /// <inheritdoc />
    public class DefaultServiceSpatial : BaseServiceSpatial<IDbContext>
    {
        /// <inheritdoc />
        public DefaultServiceSpatial(IDbContext context)
            : base(context)
        {

        }
    }
}