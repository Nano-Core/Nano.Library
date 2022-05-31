using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nano.Web.Models;
using Newtonsoft.Json;

namespace Nano.Web.Hosting.Filters
{
    /// <summary>
    /// Model State Validation Filter.
    /// </summary>
    public class ModelStateValidationFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ModelStateValidationFilter(ILogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <summary>
        /// Validates the model state upon execiting a controller action.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.ModelState.IsValid)
            {
                return;
            }

            var error = new Error
            {
                Summary = "Invalid ModelState",
                StatusCode = (int)HttpStatusCode.BadRequest,
                Exceptions = context.ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToArray()
            };

            var message = JsonConvert.SerializeObject(error);

            this.Logger
                .LogWarning(message);

            context.Result = new BadRequestObjectResult(error);
        }
    }
}