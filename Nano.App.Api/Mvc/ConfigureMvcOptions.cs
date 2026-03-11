using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Conventions;
using Nano.Data.Abstractions.Config;
using System;

namespace Nano.App.Api.Mvc;

internal sealed class ConfigureMvcOptions(IOptionsMonitor<ApiOptions> apiOptions, IOptionsMonitor<DataOptions>? dataOptions = null) : IConfigureOptions<MvcOptions>
{
    private readonly IOptionsMonitor<ApiOptions> apiOptions = apiOptions ?? throw new ArgumentNullException(nameof(apiOptions));

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
        var conditionalActionConvention = new ConditionalActionsConvention(this.apiOptions, dataOptions);

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