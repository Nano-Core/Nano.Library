using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Nano.App.Web.Mvc.Features;

/// <summary>
/// 
/// </summary>
public sealed class ConditionalControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly Func<TypeInfo, bool> predicate;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    public ConditionalControllerFeatureProvider(Func<TypeInfo, bool> predicate)
    {
        this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }

    /// <inheritdoc />
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if (parts == null) 
            throw new ArgumentNullException(nameof(parts));
        
        if (feature == null) 
            throw new ArgumentNullException(nameof(feature));

        var controllersToRemove = feature.Controllers
            .Where(x => !predicate(x))
            .ToArray();

        foreach (var controller in controllersToRemove)
        {
            feature.Controllers
                .Remove(controller);
        }
    }
}