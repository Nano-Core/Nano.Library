using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Documentation.Filters.Document;

/// <summary>
/// Swagger filter that removes routes corresponding to the default API version.
/// Useful to prevent duplicate or unnecessary default version endpoints in the Swagger documentation.
/// </summary>
public class RemoveVersionsRoutesFilter : IDocumentFilter
{
    private readonly IOptionsMonitor<ApiOptions> apiOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveVersionsRoutesFilter"/> class.
    /// </summary>
    /// <param name="apiOptions">The <see cref="IOptionsMonitor{ApiOptions}"/> containing API version and documentation settings.</param>
    public RemoveVersionsRoutesFilter(IOptionsMonitor<ApiOptions> apiOptions)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
    }

    /// <summary>
    /// Applies the filter to remove Swagger paths for the default API version based on configuration.
    /// </summary>
    /// <param name="swaggerDoc">The <see cref="OpenApiDocument"/> representing the Swagger documentation.</param>
    /// <param name="context">The <see cref="DocumentFilterContext"/> providing API descriptions.</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);
        ArgumentNullException.ThrowIfNull(context);

        if (this.apiOptions.CurrentValue.Documentation == null)
        {
            return;
        }

        var version = this.apiOptions.CurrentValue.Version
            .ParseVersion();

        var defaultVersion = new ApiVersion(version.Major, version.Minor);
        var defaultDocumentName = $"v{defaultVersion}";
        var currentDocumentName = context.DocumentName;

        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (apiDescription.RelativePath == null)
            {
                continue;
            }

            var baseRoute = $"{this.apiOptions.CurrentValue.Hosting.Root}/{currentDocumentName.Replace(".0", string.Empty)}";

            if (this.apiOptions.CurrentValue.Documentation.HideDefaultVersion && currentDocumentName == defaultDocumentName)
            {
                if (apiDescription.RelativePath.StartsWith(baseRoute, StringComparison.Ordinal))
                {
                    swaggerDoc.Paths
                        .Remove("/" + apiDescription.RelativePath);
                }
            }
        }
    }
}