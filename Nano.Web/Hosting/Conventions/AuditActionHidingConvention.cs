using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.Web.Controllers;

namespace Nano.Web.Hosting.Conventions;

/// <inheritdoc />
public class AuditActionHidingConvention : IActionModelConvention
{
    /// <inheritdoc />
    public void Apply(ActionModel action)
    {
        if (action.Controller.ControllerName == nameof(AuditController).Replace("Controller", string.Empty))
        {
            action.ApiExplorer.IsVisible = false;
        }
    }
}