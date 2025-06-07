using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.Web.Hosting.Documentation.Filters.Document;

/// <summary>
/// Lowercase Routes Filter.
/// </summary>
public class LowercaseRoutesFilter : IDocumentFilter
{
    /// <summary>
    /// Apply.
    /// </summary>
    /// <param name="swaggerDoc">The <see cref="OpenApiDocument"/>.</param>
    /// <param name="context">The <see cref="DocumentFilterContext"/>.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc == null) 
            throw new ArgumentNullException(nameof(swaggerDoc));
        
        if (context == null) 
            throw new ArgumentNullException(nameof(context));
        
        var newPaths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            newPaths
                .Add(path.Key.ToLowerInvariant(), path.Value);
        }

        swaggerDoc.Paths = newPaths;
    }
}