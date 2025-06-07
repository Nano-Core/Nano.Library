using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.Models.Const;

namespace Nano.Web.Hosting.Conventions;

/// <summary>
/// Produces Json Convention.
/// </summary>
public class ProducesJsonConvention : IActionModelConvention
{
    /// <inheritdoc />
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