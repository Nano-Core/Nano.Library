using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.App.Web.Controllers;

namespace Nano.App.Web.Hosting.Conventions;

/// <inheritdoc />
public class AuthActionHidingConvention : IActionModelConvention
{
    /// <inheritdoc />
    public void Apply(ActionModel action)
    {
        if (action.Controller.ControllerType == typeof(DefaultAuthController))
        {
            action.ApiExplorer.IsVisible = false;
        }
    }
}