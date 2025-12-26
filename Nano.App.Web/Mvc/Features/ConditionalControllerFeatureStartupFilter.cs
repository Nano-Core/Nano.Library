using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Controllers;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Config;

namespace Nano.App.Web.Mvc.Features;

/// <summary>
/// 
/// </summary>
public sealed class ConditionalControllerFeatureStartupFilter : IStartupFilter
{
    private readonly ApplicationPartManager applicationPartManager;
    private readonly IOptionsMonitor<WebOptions> webOptions;
    private readonly IOptionsMonitor<DataOptions> dataOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="applicationPartManager"></param>
    /// <param name="dataOptions"></param>
    /// <param name="webOptions"></param>
    public ConditionalControllerFeatureStartupFilter(ApplicationPartManager applicationPartManager, IOptionsMonitor<WebOptions> webOptions, IOptionsMonitor<DataOptions> dataOptions = null)
    {
        this.applicationPartManager = applicationPartManager ?? throw new ArgumentNullException(nameof(applicationPartManager));
        this.webOptions = webOptions ?? throw new ArgumentNullException(nameof(webOptions));
        this.dataOptions = dataOptions;
    }

    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        if (next == null) 
            throw new ArgumentNullException(nameof(next));
        
        return app =>
        {
            this.applicationPartManager.FeatureProviders
                .Add(new ConditionalControllerFeatureProvider(x =>
                {
                    if (x.IsTypeOf(typeof(AuditController)))
                    {
                        return (this.dataOptions?.CurrentValue.UseAudit ?? false) && webOptions.CurrentValue.Hosting.ExposeAuditController;
                    }

                    if (x.IsTypeOf(typeof(DefaultAuthController)))
                    {
                        return webOptions.CurrentValue.Hosting.ExposeAuthController;
                    }

                    return true;
                }));

            next(app);
        };
    }
}