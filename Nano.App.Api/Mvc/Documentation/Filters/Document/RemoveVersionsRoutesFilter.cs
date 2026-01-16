using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Documentation.Filters.Document;

/// <summary>
/// Remove Default Version Routes Filter.
/// </summary>
public class RemoveVersionsRoutesFilter : IDocumentFilter
{
    private readonly IOptionsMonitor<ApiOptions> webOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webOptions">The <see cref="IOptionsMonitor{ApiOptions}"/>.</param>
    public RemoveVersionsRoutesFilter(IOptionsMonitor<ApiOptions> webOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
    }

    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);
        ArgumentNullException.ThrowIfNull(context);

        if (this.webOptions.CurrentValue.Documentation == null)
        {
            return;
        }

        var version = this.webOptions.CurrentValue.Version
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

            var baseRoute = $"{this.webOptions.CurrentValue.Hosting.Root}/{currentDocumentName.Replace(".0", string.Empty)}";

            if (this.webOptions.CurrentValue.Documentation.UseDefaultVersion && currentDocumentName == defaultDocumentName)
            {
                if (apiDescription.RelativePath.StartsWith(baseRoute, StringComparison.Ordinal))
                {
                    swaggerDoc.Paths
                        .Remove("/" + apiDescription.RelativePath);
                }
            }
            else
            {
                if (!apiDescription.RelativePath.StartsWith(baseRoute, StringComparison.Ordinal))
                {
                    swaggerDoc.Paths
                        .Remove("/" + apiDescription.RelativePath);
                }
            }
        }
    }
}