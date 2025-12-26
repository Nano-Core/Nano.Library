using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Nano.App.Web.Config;
using Nano.App.Web.Controllers;
using Nano.App.Web.Mvc.Documentation.Filters.Document;
using Nano.App.Web.Mvc.Documentation.Filters.Operation;
using Nano.App.Web.Mvc.Documentation.Filters.Schema;
using Nano.Common.Helpers;
using Swashbuckle.AspNetCore.SwaggerGen;
using TypeExtensions = Nano.Common.Extensions.TypeExtensions;

namespace Nano.App.Web.Mvc.Documentation.Config;

/// <summary>
/// Configure Swagger Options.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;
    private readonly IOptionsMonitor<WebOptions> webOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="apiVersionDescriptionProvider">The <see cref="IApiVersionDescriptionProvider"/>.</param>
    /// <param name="webOptions">The <see cref="WebOptions"/>.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider, IOptionsMonitor<WebOptions> webOptions)
    {
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider ?? throw new ArgumentNullException(nameof(apiVersionDescriptionProvider));
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        foreach (var provider in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = webOptions.CurrentValue.Name,
                Description = webOptions.CurrentValue.Documentation.Description,
                Version = provider.ApiVersion.ToString(),
            };

            if (webOptions.CurrentValue.Documentation.Contact != null)
            {
                info.Contact = webOptions.CurrentValue.Documentation.Contact;
            }

            if (webOptions.CurrentValue.Documentation.License != null)
            {
                info.License = webOptions.CurrentValue.Documentation.License;
            }

            if (!string.IsNullOrEmpty(webOptions.CurrentValue.Documentation.TermsOfService))
            {
                info.TermsOfService = new Uri(webOptions.CurrentValue.Documentation.TermsOfService);
            }

            if (provider.IsDeprecated)
            {
                info.Description += " This version has been deprecated.";
            }

            options
                .SwaggerDoc(provider.GroupName, info);
        }

        options
            .IgnoreObsoleteActions();
        
        options
            .IgnoreObsoleteProperties();
        
        options
            .EnableAnnotations(true, true);
        
        options
            .CustomSchemaIds(y => y.FullName);
        
        options
            .OrderActionsBy(y => y.RelativePath);

        options
            .SchemaFilter<EnumsFilter>();
        
        options
            .SchemaFilter<ResponseOnlyFilter>();
        
        options
            .OperationFilter<SwaggerResponseOnlyFilter>();
        
        options
            .DocumentFilter<RemoveVersionsRoutesFilter>();

        var openApiSecurityRequirement = new OpenApiSecurityRequirement();

        if (webOptions.CurrentValue.Identity?.Authentication.Jwt != null)
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Format: Authorization: Bearer [token]"
            };

            options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            openApiSecurityRequirement
                .Add(jwtSecurityScheme, new List<string>());
        }

        if (webOptions.CurrentValue.Identity?.Authentication.ApiKey != null)
        {
            var apiKeySecurityScheme = new OpenApiSecurityScheme
            {
                Name = "x-api-key",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = "API Key needed to access endpoints."
            };

            options
                .AddSecurityDefinition("Api-Key", apiKeySecurityScheme);

            openApiSecurityRequirement
                .Add(apiKeySecurityScheme, new List<string>());
        }

        options.AddSecurityRequirement(openApiSecurityRequirement);

        TypesHelper.GetAllTypes()
            .Where(y => TypeExtensions.IsTypeOf(y, typeof(BaseController)))
            .Select(y => y.Module)
            .Distinct()
            .ToList()
            .ForEach(y =>
            {
                var name = y.Name.Replace(".dll", ".xml").Replace(".exe", ".xml");
                var path = Path.Combine(AppContext.BaseDirectory, name);

                if (File.Exists(path))
                {
                    options
                        .IncludeXmlComments(path, true);
                }

                var modelsName = y.Name.Replace(".dll", "").Replace(".exe", "") + ".Models.xml";
                var modelsPath = Path.Combine(AppContext.BaseDirectory, modelsName);

                if (File.Exists(modelsPath))
                {
                    options
                        .IncludeXmlComments(modelsPath);
                }

                y.Assembly
                    .GetManifestResourceNames()
                    .Where(z => z.ToLower().EndsWith(".xml"))
                    .ToList()
                    .ForEach(z =>
                    {
                        var resource = y.Assembly
                            .GetManifestResourceStream(z);

                        if (resource != null)
                        {
                            options
                                .IncludeXmlComments(() => new XPathDocument(resource));
                        }
                    });
            });

        // BUG: 000: Swagger hide controllers and actions (Auth, Transient Auth, Identity and maybe also Audit?)
        //options
        //    .DocInclusionPredicate((docName, apiDesc) =>
        //    {
        //        var controller = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        //        if (controller != null)
        //        {
        //            if (controller.ControllerTypeInfo == typeof(AuditController))
        //            {
        //                return false;
        //            }

        //            if (controller.ControllerTypeInfo == typeof(DefaultAuthController) && controller.ActionName == "HiddenAction")
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    });


    }
}