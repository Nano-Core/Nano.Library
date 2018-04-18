using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Nano.Web.Hosting.Conventions
{
    /// <inheritdoc />
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel prefix;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateProvider">The <see cref="IRouteTemplateProvider"/>.</param>
        public RoutePrefixConvention(IRouteTemplateProvider templateProvider)
        {
            if (templateProvider == null)
                throw new ArgumentNullException(nameof(templateProvider));

            this.prefix = new AttributeRouteModel(templateProvider);
        }

        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var matched = controller.Selectors.Where(x => x.AttributeRouteModel != null);
                foreach (var model in matched)
                {
                    model.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(prefix, model.AttributeRouteModel);
                }

                var unmatched = controller.Selectors.Where(x => x.AttributeRouteModel == null);
                foreach (var model in unmatched)
                {
                    model.AttributeRouteModel = prefix;
                }
            }
        }
    }
}