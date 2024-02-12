using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.Web.Controllers;

namespace Nano.Web.Hosting.Conventions;

/// <inheritdoc />
public class AuthActionHidingConvention : IActionModelConvention
{
    /// <inheritdoc />
    public void Apply(ActionModel action)
    {
        if (action.Controller.ControllerName == nameof(DefaultAuthController).Replace("Controller", string.Empty))
        {
            action.ApiExplorer.IsVisible = false;
        }
    }
}