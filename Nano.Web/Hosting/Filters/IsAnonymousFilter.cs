using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nano.Web.Hosting.Filters;

/// <summary>
/// Is Anonymous Filter.
/// </summary>
public class IsAnonymousFilter : ActionFilterAttribute
{
    /// <summary>
    /// Add IsAnonymous key/value pair to <see cref="HttpContext.Items"/>.
    /// </summary>
    /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var isAnonymous = false;

        if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            var actionAttribute = descriptor.MethodInfo
                .GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                .FirstOrDefault();

            if (actionAttribute != null)
            {
                isAnonymous = true;
            }
            else
            {
                var controllerAttribute = descriptor.ControllerTypeInfo
                    .GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .FirstOrDefault();

                if (controllerAttribute != null)
                {
                    isAnonymous = true;
                }
            }
        }

        context.HttpContext.Items
            .Add("IsAnonymous", isAnonymous);
    }
}