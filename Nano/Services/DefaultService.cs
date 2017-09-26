using Microsoft.EntityFrameworkCore;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultService<TContext> : BaseService<TContext>
        where TContext : DbContext
    {
        /// <inheritdoc />
        public DefaultService(TContext context)
            : base(context)
        {

        }
    }
}