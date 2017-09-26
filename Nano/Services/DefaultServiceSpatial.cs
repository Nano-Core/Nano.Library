using Microsoft.EntityFrameworkCore;

namespace Nano.Services
{
    /// <inheritdoc />
    public class DefaultServiceSpatial<TContext> : BaseServiceSpatial<TContext>
        where TContext : DbContext
    {
        /// <inheritdoc />
        public DefaultServiceSpatial(TContext context)
            : base(context)
        {

        }

    }
}