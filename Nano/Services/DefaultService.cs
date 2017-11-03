using Nano.Config.Providers.Eventing.Interfaces;
using Nano.Data.Interfaces;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultService : BaseService<IDbContext, IEventingProvider>
    {
        /// <inheritdoc />
        public DefaultService(IDbContext context, IEventingProvider eventing)
            : base(context, eventing)
        {

        }
    }
}