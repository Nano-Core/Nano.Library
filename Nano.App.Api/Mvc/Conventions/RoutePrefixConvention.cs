using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Nano.App.Api.Mvc.Conventions;

/// <summary>
/// Convention to apply a common route prefix to all controllers.
/// </summary>
public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutePrefixConvention"/> class.
    /// </summary>
    /// <param name="templateProvider">The <see cref="IRouteTemplateProvider"/> that defines the route prefix.</param>
    public RoutePrefixConvention(IRouteTemplateProvider templateProvider)
    {
        ArgumentNullException.ThrowIfNull(templateProvider);

        this.prefix = new AttributeRouteModel(templateProvider);
    }

    /// <summary>
    /// Applies the route prefix to all controllers in the application.
    /// </summary>
    /// <param name="application">The <see cref="ApplicationModel"/> containing the controllers.</param>
    public void Apply(ApplicationModel application)
    {
        ArgumentNullException.ThrowIfNull(application);

        foreach (var controller in application.Controllers)
        {
            var matched = controller.Selectors
                .Where(x => x.AttributeRouteModel != null);

            foreach (var model in matched)
            {
                model.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(this.prefix, model.AttributeRouteModel);
            }

            var unmatched = controller.Selectors
                .Where(x => x.AttributeRouteModel == null);

            foreach (var model in unmatched)
            {
                model.AttributeRouteModel = this.prefix;
            }
        }
    }
}