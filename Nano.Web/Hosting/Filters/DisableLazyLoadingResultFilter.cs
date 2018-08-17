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
        /// The <see cref="IService"/>.
        /// </summary>
        protected virtual IService Service { get; }

        /// <summary>
        /// Constructor.
        /// Initialzing the <see cref="IService"/> with the passed <paramref name="service"/>.
        /// </summary>
        /// <param name="service">The <see cref="IService"/>.</param>
        public DisableLazyLoadingResultFilterAttribute(IService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this.Service = service;
        }

        /// <inheritdoc />
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            this.Service.IsLazyLoadingEnabled = false;

            base.OnResultExecuting(context);
        }
    }
}