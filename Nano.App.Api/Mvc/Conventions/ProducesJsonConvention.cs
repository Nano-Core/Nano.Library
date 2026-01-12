using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nano.App.ApiClient.Consts;

namespace Nano.App.Api.Mvc.Conventions;

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