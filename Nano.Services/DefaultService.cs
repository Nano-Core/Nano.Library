using Nano.Data;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultService : BaseService<DefaultDbContext>
    {
        /// <inheritdoc />
        public DefaultService(DefaultDbContext context)
            : base(context)
        {

        }
    }
}