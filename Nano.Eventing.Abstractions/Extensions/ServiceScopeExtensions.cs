using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Eventing.Abstractions.Extensions;

/// <summary>
/// Service scope extensions to automatically register and subscribe all event handlers during app startup.
/// </summary>
public static class ServiceScopeExtensions
{
    /// <summary>
    /// Scans for all implementations of <see cref="IEventingHandler{TEvent}"/> in the application,
    /// resolves them from the DI container, and subscribes them to the configured <see cref="IEventing"/> instance.
    /// <para>
    ///     Should be called once during application startup (e.g., in <c>Program.cs</c> or <c>Startup.Configure</c>).
    /// </para>
    /// </summary>
    /// <param name="serviceScope">The <see cref="IServiceScope"/>.</param>
    /// <param name="rootServiceProvider">The root <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceScope"/> instance, for chaining.</returns>
    public static IServiceScope UseEventHandlers(this IServiceScope serviceScope, IServiceProvider rootServiceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceScope);

        var registerEventHandlersTask = serviceScope.ServiceProvider
            .GetService<IRegisterEventingHandlersTask>();

        registerEventHandlersTask?
            .RegisterEventHandlers(serviceScope, rootServiceProvider)
            .GetAwaiter()
            .GetResult();

        return serviceScope;
    }
}