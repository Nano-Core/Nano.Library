using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.Web.Controllers;

namespace Nano.Web.Hosting.Conventions
{
    /// <inheritdoc />
    public class AduitControllerDisabledConvention : IApplicationModelConvention
    {
        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.ControllerName == nameof(AuditController).Replace("Controller", string.Empty))
                    controller.ApiExplorer.IsVisible = false;
            }
        }
    }
}