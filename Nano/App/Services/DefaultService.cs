using Nano.Data.Interfaces;

namespace Nano.App.Services
{
    /// <inheritdoc />
    public class DefaultService : BaseService<IDbContext>
    {
        /// <inheritdoc />
        public DefaultService(IDbContext context)
            : base(context)
        {

        }
    }
}