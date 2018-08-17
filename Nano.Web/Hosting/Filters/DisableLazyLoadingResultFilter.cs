using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Nano.Web.Hosting.Filters
{
    /// <summary>
    /// A <see cref="ResultFilterAttribute"/> that disables lazy loading before serialzing the response.
    /// </summary>
    public class DisableLazyLoadingResultFilterAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// The <see cref="DbContext"/>.
        /// </summary>
        protected virtual DbContext DbContext { get; }

        /// <summary>
        /// Constructor.
        /// Initialzing the <see cref="DbContext"/> with the passed <paramref name="dbContext"/>.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        public DisableLazyLoadingResultFilterAttribute(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <inheritdoc />
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            this.DbContext.ChangeTracker.LazyLoadingEnabled = false;

            base.OnResultExecuting(context);
        }
    }
}