using Nano.Config.Providers.Eventing.Interfaces;
using Nano.Data.Interfaces;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultServiceSpatial : BaseServiceSpatial<IDbContext, IEventingProvider>
    {
        /// <inheritdoc />
        public DefaultServiceSpatial(IDbContext context, IEventingProvider eventing)
            : base(context, eventing)
        {

        }
    }
}