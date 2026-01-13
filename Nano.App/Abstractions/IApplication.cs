using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nano.App.Abstractions;

/// <summary>
/// Application.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    public IApplication ConfigureServices(Action<IServiceCollection> configure);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IApplication Build();

    /// <summary>
    /// 
    /// </summary>
    public void Run();
}