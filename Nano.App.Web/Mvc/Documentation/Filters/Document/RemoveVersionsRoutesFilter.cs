using System;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nano.App.Web.Config;
using Nano.Common.Config;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Web.Mvc.Documentation.Filters.Document;

/// <summary>
/// Remove Default Version Routes Filter.
/// </summary>
public class RemoveVersionsRoutesFilter : IDocumentFilter
{
    private readonly IOptionsMonitor<WebOptions> webOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="webOptions">The <see cref="IOptionsMonitor{WebOptions}"/>.</param>
    public RemoveVersionsRoutesFilter(IOptionsMonitor<WebOptions> webOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
    }

    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc == null)
            throw new ArgumentNullException(nameof(swaggerDoc));

        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var defaultVersion = new ApiVersion(ConfigManager.Version.Major, ConfigManager.Version.Minor == 0 ? null : ConfigManager.Version.Minor);

        var defaultDocumentName = $"v{defaultVersion}";
        var currentDocumentName = context.DocumentName.Replace(".0", string.Empty);

        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (apiDescription.RelativePath == null)
            {
                continue;
            }

            var baseRoute = $"{this.webOptions.CurrentValue.Hosting.Root}/{currentDocumentName}";

            if (this.webOptions.CurrentValue.Documentation.UseDefaultVersion && currentDocumentName == defaultDocumentName)
            {
                if (apiDescription.RelativePath.StartsWith(baseRoute))
                {
                    swaggerDoc.Paths
                        .Remove("/" + apiDescription.RelativePath);
                }
            }
            else
            {
                if (!apiDescription.RelativePath.StartsWith(baseRoute))
                {
                    swaggerDoc.Paths
                        .Remove("/" + apiDescription.RelativePath);
                }
            }
        }
    }
}