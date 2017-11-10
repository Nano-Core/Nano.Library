using Nano.Data.Interfaces;
using Nano.Eventing.Interfaces;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultService : BaseService<IDbContext, IEventing>
    {
        /// <inheritdoc />
        public DefaultService(IDbContext context, IEventing eventing)
            : base(context, eventing)
        {

        }
    }
}