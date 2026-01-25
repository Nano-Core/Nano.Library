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
/// Schema filter that removes properties marked with <see cref="RequestIgnoreAttribute"/> from request models in Swagger documentation.
/// </summary>
public sealed class RequestIgnoreSchemaFilter : ISchemaFilter
{
    private readonly IApiDescriptionGroupCollectionProvider apiDescriptionProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIgnoreSchemaFilter"/> class.
    /// </summary>
    /// <param name="apiDescriptionProvider">The API description provider used to determine if a type is used in request bodies.</param>
    public RequestIgnoreSchemaFilter(IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
    {
        this.apiDescriptionProvider = apiDescriptionProvider ?? throw new ArgumentNullException(nameof(apiDescriptionProvider));
    }

    /// <summary>
    /// Applies the filter to remove properties marked with <see cref="RequestIgnoreAttribute"/> from the schema.
    /// </summary>
    /// <param name="schema">The <see cref="IOpenApiSchema"/> to modify.</param>
    /// <param name="context">The <see cref="SchemaFilterContext"/> providing type information.</param>
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