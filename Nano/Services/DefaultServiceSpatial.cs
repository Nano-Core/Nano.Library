using Nano.Data.Interfaces;
using Nano.Eventing.Interfaces;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultServiceSpatial : BaseServiceSpatial<IDbContext, IEventing>
    {
        /// <inheritdoc />
        public DefaultServiceSpatial(IDbContext context, IEventing eventing)
            : base(context, eventing)
        {

        }
    }
}