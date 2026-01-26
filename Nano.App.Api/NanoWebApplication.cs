using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Abstractions;

namespace Nano.App.Api;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TApp"></typeparam>
public sealed class NanoWebApplication<TApp> : NanoApiApplication
    where TApp : class, IComponent
{
    private NanoWebApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public override IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        base.ConfigureServices(configure);

        this.applicationBuilder.Services
            .AddRazorPages();

        this.applicationBuilder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        //applicationBuilder.Services
        //    .AddRazorPages();
        //applicationBuilder.Services
        //    .AddControllersWithViews();
        //applicationBuilder.Services
        //    .AddRazorComponents()
        //    .AddInteractiveServerComponents();
        //applicationBuilder.Services
        //    .AddServerSideBlazor();

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public new static IApplication ConfigureApp(params string[] args)
    {
        var builder = CreateBuilder(args);

        return new NanoWebApplication<TApp>(builder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new IApplication Build()
    {
        base.Build();

        this.application
            .MapRazorComponents<TApp>()
            .AddInteractiveServerRenderMode();

        return this;
    }
}