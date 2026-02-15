using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Conventions;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Config;

namespace Nano.App.Api.Mvc;

/// <summary>
/// Configures MVC options such as conventions, filters, formatters, and HTTPS requirements.
/// </summary>
public sealed class ConfigureMvcOptions : IConfigureOptions<MvcOptions>
{
    private readonly IOptionsMonitor<ApiOptions> apiOptions;
    private readonly IOptionsMonitor<DataOptions> dataOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureMvcOptions"/> class.
    /// </summary>
    /// <param name="apiOptions">The <see cref="IOptionsMonitor{ApiOptions}"/> for web API configuration.</param>
    /// <param name="dataOptions">The <see cref="IOptionsMonitor{DataOptions}"/> for data configuration.</param>
    public ConfigureMvcOptions(IOptionsMonitor<ApiOptions> apiOptions, IOptionsMonitor<DataOptions> dataOptions)
    {
        this.apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));
        this.dataOptions = dataOptions;
    }

    /// <summary>
    /// Configures the <see cref="MvcOptions"/> including conventions, filters, and formatters.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions"/> to configure.</param>
    public void Configure(MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ReturnHttpNotAcceptable = true;
        options.RespectBrowserAcceptHeader = true;
        options.MaxValidationDepth = 128;

        var routeAttribute = new RouteAttribute(this.apiOptions.CurrentValue.Hosting.Root);
        var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
        var producesJsonConvention = new ProducesJsonConvention();
        var conditionalActionConvention = new ConditionalActionsConvention(this.apiOptions, this.dataOptions);

        options.Conventions
            .Insert(0, routePrefixConvention);

        options.Conventions
            .Add(producesJsonConvention);

        options.Conventions
            .Add(conditionalActionConvention);

        if (this.apiOptions.CurrentValue.Hosting.Https is { UseHttpsRequired: true })
        {
            options.Filters
                .Add<RequireHttpsAttribute>();
        }
    }
}