using Nano.Data.Interfaces;
using Nano.Eventing.Providers.Interfaces;

namespace Nano.App.Services
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