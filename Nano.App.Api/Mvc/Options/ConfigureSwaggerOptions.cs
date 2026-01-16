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
/// Configure Swagger Options.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    //private static readonly Regex curlyBracketsRegex = new(@"\{([^}]+)\}", RegexOptions.Compiled);
    private static readonly string[] xmlEmbeddedResources =
    [
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.App.Web.xml",
        "Nano.App.Web.Mvc.Documentation..xmldoc.Nano.Data.Abstractions.xml"
    ];

    private readonly IOptionsMonitor<ApiOptions> webOptions;
    private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="authenticationSchemeProvider"></param>
    /// <param name="apiVersionDescriptionProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ConfigureSwaggerOptions(IOptionsMonitor<ApiOptions> webOptions, IAuthenticationSchemeProvider authenticationSchemeProvider, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider ?? throw new ArgumentNullException(nameof(apiVersionDescriptionProvider));
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.ConfigureApiInfos(options);
        this.ConfigureSecurityDefinitions(options);
        ConfigureDocumentationSources(options);

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

                if (id.StartsWith(this.webOptions.CurrentValue.Hosting.Root, StringComparison.Ordinal))
                {
                    id = id[this.webOptions.CurrentValue.Hosting.Root.Length..];
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

        if (this.webOptions.CurrentValue.Documentation == null)
        {
            return;
        }

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
    private static void ConfigureDocumentationSources(SwaggerGenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

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
                .IncludeXmlComments(file, true);
        }
    }
}