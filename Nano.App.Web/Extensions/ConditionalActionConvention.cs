using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Nano.App.Web.Extensions;

// BUG: 000: Swagger hide controllers and actions

/// <summary>
/// 
/// </summary>
public class ConditionalActionConvention : IControllerModelConvention
{
    private readonly Func<ControllerModel, ActionModel, bool> _predicate;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    public ConditionalActionConvention(Func<ControllerModel, ActionModel, bool> predicate)
    {
        _predicate = predicate;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controller"></param>
    public void Apply(ControllerModel controller)
    {
        var actionsToRemove = controller.Actions
            .Where(a => !_predicate(controller, a))
            .ToList();

        foreach (var action in actionsToRemove)
            controller.Actions.Remove(action);
    }
}