using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Nano.App.Web.Config;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Web.Mvc.Documentation.Filters.Document;

internal static class VersionExtensions
{
    internal static Version ParseVersion(this string value)
    {
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        if (value == string.Empty)
        {
            return new Version(1, 0);
        }

        if (Version.TryParse(value, out var version))
        {
            return version;
        }

        if (int.TryParse(value, out var major))
        {
            return new Version(major, 0);
        }

        throw new FormatException($"Invalid version format: '{value}'. Expected 'major.minor'.");
    }
}

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