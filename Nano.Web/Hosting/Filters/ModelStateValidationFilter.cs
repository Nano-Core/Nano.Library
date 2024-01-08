using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nano.Models;

namespace Nano.Web.Hosting.Filters;

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

        var exceptions = context.ModelState.Values
            .SelectMany(x => x.Errors
                .Select(y => y.ErrorMessage))
            .ToArray();

        var error = new Error
        {
            Summary = "Invalid ModelState",
            Exceptions = exceptions
        };

        foreach (var exception in error.Exceptions)
        {
            this.Logger
                .LogWarning($"{error.Summary}{exception}");
        }

        context.Result = new BadRequestObjectResult(error);
    }
}