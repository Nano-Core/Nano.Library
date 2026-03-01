using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Eventing.Abstractions.Extensions;

/// <summary>
/// Service provider extensions to automatically register and subscribe all event handlers during app startup.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Scans for all implementations of <see cref="IEventingHandler{TEvent}"/> in the application,
    /// resolves them from the DI container, and subscribes them to the configured <see cref="IEventing"/> instance.
    /// <para>
    ///     Should be called once during application startup (e.g., in <c>Program.cs</c> or <c>Startup.Configure</c>).
    /// </para>
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>The same <see cref="IServiceProvider"/> instance, for chaining.</returns>
    public static IServiceProvider UseEventHandlers(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider
            .CreateScope();

        var registerEventHandlersTask = scope.ServiceProvider
            .GetService<IRegisterEventHandlersTask>();

        registerEventHandlersTask?
            .RegisterEventHandlers(scope.ServiceProvider)
            .GetAwaiter()
            .GetResult();

        return serviceProvider;
    }
}