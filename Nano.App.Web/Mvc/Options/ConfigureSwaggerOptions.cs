using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Nano.App.Web.Config;
using Nano.App.Web.Mvc.Documentation.Filters.Document;
using Nano.App.Web.Mvc.Documentation.Filters.Schema;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Nano.Common.Extensions;

namespace Nano.App.Web.Mvc.Options;

/// <summary>
/// Configure Swagger Options.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private static readonly string[] xmlEmbeddedResources =
    [
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.Web.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.Data.Abstractions.xml"
    ];

    private readonly IOptionsMonitor<WebOptions> webOptions;
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="authenticationSchemeProvider"></param>
    /// <param name="apiVersionDescriptionProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConfigureSwaggerOptions(IOptionsMonitor<WebOptions> webOptions, IAuthenticationSchemeProvider authenticationSchemeProvider, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider ?? throw new ArgumentNullException(nameof(apiVersionDescriptionProvider));
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

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
            .SchemaFilter<EnumsFilter>();

        options
            .SchemaFilter<RequestIgnoreSchemaFilter>();

        options
            .DocumentFilter<RemoveVersionsRoutesFilter>();
    }


    private void ConfigureApiInfos(SwaggerGenOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = this.webOptions.CurrentValue.Name,
                Description = this.webOptions.CurrentValue.Documentation.Description,
                Version = apiVersionDescription.ApiVersion.ToString()
            };

            if (this.webOptions.CurrentValue.Documentation.Contact != null)
            {
                openApiInfo.Contact = this.webOptions.CurrentValue.Documentation.Contact;
            }

            if (this.webOptions.CurrentValue.Documentation.License != null)
            {
                openApiInfo.License = this.webOptions.CurrentValue.Documentation.License;
            }

            if (!string.IsNullOrEmpty(this.webOptions.CurrentValue.Documentation.TermsOfService))
            {
                openApiInfo.TermsOfService = new Uri(this.webOptions.CurrentValue.Documentation.TermsOfService);
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
        if (options == null)
            throw new ArgumentNullException(nameof(options));

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
        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var manifestResourceNames = assembly
                .GetManifestResourceNames();

            foreach (var resourceName in xmlEmbeddedResources)
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
                .IncludeXmlComments(file, includeControllerXmlComments: true);
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
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "members")
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