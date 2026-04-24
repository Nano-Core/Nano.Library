using System;
using Microsoft.AspNetCore.Builder;
using Nano.App.Api.Extensions;
using Nano.App.Web.Config;

namespace Nano.App.Web.Extensions;

internal static class WebApplicationExtensions
{
    internal static WebApplication ConfigureNanoWebApplication<TRoot>(this WebApplication webApplication, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        ArgumentNullException.ThrowIfNull(options);

        webApplication
            .ConfigureNanoApiApplication(options);

        webApplication
            .UseNanoRazor<TRoot>(options)
            .UseNanoBlazor(options);

        return webApplication;
    }

    internal static WebApplication UseNanoRazor<TRoot>(this WebApplication webApplication, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        ArgumentNullException.ThrowIfNull(options);

        webApplication
            .MapRazorPages();

        webApplication
            .MapRazorComponents<TRoot>()
            .AddInteractiveServerRenderMode();

        return webApplication;
    }

    internal static IApplicationBuilder UseNanoBlazor(this WebApplication webApplication, WebOptions options)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        ArgumentNullException.ThrowIfNull(options);

        webApplication
            .MapBlazorHub();

        return webApplication;
    }
}