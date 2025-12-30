using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Consts;
using Nano.App.Web.Config;
using Nano.App.Web.Mvc.Conventions;
using Nano.Data.Abstractions.Config;

namespace Nano.App.Web.Mvc.Options;

/// <summary>
/// 
/// </summary>
public sealed class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
    private readonly IOptionsMonitor<WebOptions> webOptions;
    private readonly IOptionsMonitor<DataOptions> dataOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webOptions"></param>
    /// <param name="dataOptions"></param>
    public ConfigureMvcOptions(IOptionsMonitor<WebOptions> webOptions, IOptionsMonitor<DataOptions> dataOptions)
    {
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.dataOptions = dataOptions;
    }

    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        if (options == null) 
            throw new ArgumentNullException(nameof(options));
        
        options.ReturnHttpNotAcceptable = true;
        options.RespectBrowserAcceptHeader = true;
        options.MaxValidationDepth = 128;

        options.FormatterMappings
            .SetMediaTypeMappingForFormat("json", HttpContentType.JSON);

        var routeAttribute = new RouteAttribute(this.webOptions.CurrentValue.Hosting.Root);
        var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
        var producesJsonConvention = new ProducesJsonConvention();
        var conditionalActionConvention = new ConditionalActionsConvention(this.webOptions, this.dataOptions);

        options.Conventions
            .Insert(0, routePrefixConvention);

        options.Conventions
            .Add(producesJsonConvention);

        options.Conventions
            .Add(conditionalActionConvention);

        if (this.webOptions.CurrentValue.Hosting.UseHttpsRequired)
        {
            options.Filters
                .Add<RequireHttpsAttribute>();
        }
    }
}