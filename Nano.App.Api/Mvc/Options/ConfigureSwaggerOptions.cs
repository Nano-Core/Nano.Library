using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Documentation.Filters.Document;
using Nano.App.Api.Mvc.Documentation.Filters.Schema;
using Nano.App.Api.Mvc.Options.Regex;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nano.App.Api.Mvc.Options;

/// <summary>
/// Configures Swagger generation options including API documentation, security definitions, and XML comments.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private static readonly string[] xmlEmbeddedResources =
    [
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.Web.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.Data.Abstractions.xml"
    ];

    private readonly IOptionsMonitor<ApiOptions> apiOptions;
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="apiOptions">The <see cref="IOptionsMonitor{ApiOptions}"/> for API configuration.</param>
    /// <param name="authenticationSchemeProvider">The <see cref="IAuthenticationSchemeProvider"/> to retrieve authentication schemes.</param>
    /// <param name="apiVersionDescriptionProvider">The <see cref="IApiVersionDescriptionProvider"/> for API version information.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
    public ConfigureSwaggerOptions(IOptionsMonitor<ApiOptions> apiOptions, IAuthenticationSchemeProvider authenticationSchemeProvider, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
        this.authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider ?? throw new ArgumentNullException(nameof(apiVersionDescriptionProvider));
    }

    /// <summary>
    /// Configures the <see cref="SwaggerGenOptions"/> including API info, security definitions, schema and document filters.
    /// </summary>
    /// <param name="options">The <see cref="SwaggerGenOptions"/> to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.ConfigureApiInfos(options);
        this.ConfigureSecurityDefinitions(options);
        this.ConfigureDocumentationSources(options);

        options
            .IgnoreObsoleteActions();

        options
            .IgnoreObsoleteProperties();

        options
            .EnableAnnotations(true, true);

        options
            .CustomSchemaIds(y => y.GetFriendlyName());

        options
            .OrderActionsBy(y => y.RelativePath);

        options
            .CustomOperationIds(x =>
            {
                var httpMethod = x.HttpMethod?
                    .ToLower();

                var id = x.RelativePath?
                    .Replace("/", "-");

                if (id == null)
                {
                    return null;
                }

                if (id.StartsWith(this.apiOptions.CurrentValue.Hosting.Root, StringComparison.Ordinal))
                {
                    id = id[this.apiOptions.CurrentValue.Hosting.Root.Length..];
                }

                id = Regexes.CurlyBrackets()
                    .Replace(id, "$1");

                return $"{httpMethod}{id}";
            });

        options
            .SchemaFilter<EnumsFilter>();

        options
            .SchemaFilter<RequestIgnoreSchemaFilter>();

        options
            .DocumentFilter<RemoveVersionsRoutesFilter>();
    }


    private void ConfigureApiInfos(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (this.apiOptions.CurrentValue.Documentation == null)
        {
            return;
        }

        foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = this.apiOptions.CurrentValue.Documentation.Name,
                Description = this.apiOptions.CurrentValue.Documentation.Description,
                Version = apiVersionDescription.ApiVersion.ToString()
            };

            if (this.apiOptions.CurrentValue.Documentation.Contact != null)
            {
                openApiInfo.Contact = this.apiOptions.CurrentValue.Documentation.Contact;
            }

            if (this.apiOptions.CurrentValue.Documentation.License != null)
            {
                openApiInfo.License = this.apiOptions.CurrentValue.Documentation.License;
            }

            if (!string.IsNullOrEmpty(this.apiOptions.CurrentValue.Documentation.TermsOfService))
            {
                openApiInfo.TermsOfService = new Uri(this.apiOptions.CurrentValue.Documentation.TermsOfService);
            }

            if (apiVersionDescription.IsDeprecated)
            {
                openApiInfo.Description += " This version has been deprecated.";
            }

            options
                .SwaggerDoc(apiVersionDescription.GroupName, openApiInfo);
        }
    }
    private void ConfigureSecurityDefinitions(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var authenticationSchemes = this.authenticationSchemeProvider
            .GetAllSchemesAsync()
            .GetAwaiter()
            .GetResult()
            .Select(x => x.Name)
            .ToArray();

        if (authenticationSchemes.Contains(AuthenticationSchemes.JWT))
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = AuthenticationSchemes.JWT,
                Description = "JWT Authorization header using the Bearer scheme. Format: Authorization: Bearer [token]",
                Scheme = "Bearer"
            };

            options
                .AddSecurityDefinition(AuthenticationSchemes.JWT, jwtSecurityScheme);
        }

        if (authenticationSchemes.Contains(AuthenticationSchemes.API_KEY))
        {
            var apiKeySecurityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = AuthenticationSchemes.API_KEY,
                Description = "API Key Authorization headering using api-key scheme. Format: X-Api-Key: [apikey]",
                Scheme = ApiKeyHeaderNames.X_API_KEY
            };

            options
                .AddSecurityDefinition("ApiKey", apiKeySecurityScheme);
        }
    }
    private void ConfigureDocumentationSources(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // BUG: Is .xmldoc embedded files still needed?

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var manifestResourceNames = assembly
                .GetManifestResourceNames();

            foreach (var resourceName in ConfigureSwaggerOptions.xmlEmbeddedResources)
            {
                var contains = manifestResourceNames
                    .Contains(resourceName);

                if (!contains)
                {
                    continue;
                }

                var stream = assembly
                    .GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    continue;
                }

                options
                    .IncludeXmlComments(() => new XPathDocument(stream));
            }
        }

        foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
        {
            if (!IsCSharpXmlDoc(file))
            {
                continue;
            }

            options
                .IncludeXmlComments(file, true);
        }
    }

    private static bool IsCSharpXmlDoc(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);

            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreWhitespace = true
            });

            reader
                .MoveToContent();

            if (!reader.IsStartElement("doc"))
            {
                return false;
            }

            reader
                .ReadStartElement("doc");

            while (reader.Read())
            {
                if (reader is { NodeType: XmlNodeType.Element, Name: "members" })
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}