using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.Common.Consts;

namespace Nano.App.Api.Mvc.Conventions;

/// <summary>
/// Convention to ensure that all actions produce JSON responses.
/// </summary>
public class ProducesJsonConvention : IActionModelConvention
{
    /// <summary>
    /// Applies the JSON response convention to the given action.
    /// </summary>
    /// <param name="action">The <see cref="ActionModel"/> to apply the convention to.</param>
    public void Apply(ActionModel action)
    {
        var producesAttribute = action.Attributes
            .OfType<ProducesAttribute>()
            .FirstOrDefault();

        if (producesAttribute != null && !producesAttribute.ContentTypes.Contains(HttpContentType.JSON))
        {
            producesAttribute.ContentTypes
                .Add(HttpContentType.JSON);
        }
    }
}