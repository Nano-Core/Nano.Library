using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi;
using Nano.Data.Abstractions.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Documentation.Filters.Schema;

/// <summary>
/// 
/// </summary>
public sealed class RequestIgnoreSchemaFilter : ISchemaFilter
{
    private readonly IApiDescriptionGroupCollectionProvider apiDescriptionProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiDescriptionProvider"></param>
    public RequestIgnoreSchemaFilter(IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
    {
        this.apiDescriptionProvider = apiDescriptionProvider ?? throw new ArgumentNullException(nameof(apiDescriptionProvider));
    }

    /// <inheritdoc />
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null || context.Type == null)
        {
            return;
        }

        if (!IsUsedInRequest(context.Type))
        {
            return;
        }

        var propertiesToRemove = context.Type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute<RequestIgnoreAttribute>() != null)
            .Select(x => x.Name);

        foreach (var prop in propertiesToRemove)
        {
            schema.Properties.Remove(prop);
        }
    }


    private bool IsUsedInRequest(Type modelType)
    {
        return this.apiDescriptionProvider.ApiDescriptionGroups.Items
            .SelectMany(x => x.Items)
            .SelectMany(x => x.ParameterDescriptions)
            .Any(x => x.Source == BindingSource.Body && x.Type == modelType);
    }
}