using Nano.Data.Interfaces;
using Nano.Eventing.Providers.Interfaces;

namespace Nano.App.Services
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