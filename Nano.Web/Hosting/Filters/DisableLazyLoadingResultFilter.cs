using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Nano.Services.Interfaces;

namespace Nano.Web.Hosting.Filters
{
    /// <summary>
    /// A <see cref="ResultFilterAttribute"/> that disables lazy loading before serialzing the response.
    /// </summary>
    public class DisableLazyLoadingResultFilterAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// The <see cref="IRepository"/>.
        /// </summary>
        protected virtual IRepository Repository { get; }

        /// <summary>
        /// Constructor.
        /// Initialzing the <see cref="IRepository"/> with the passed <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository"/>.</param>
        public DisableLazyLoadingResultFilterAttribute(IRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            this.Repository = repository;
        }

        /// <inheritdoc />
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            this.Repository.IsLazyLoadingEnabled = false;

            base.OnResultExecuting(context);
        }
    }
}